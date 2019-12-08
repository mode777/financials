using Finances.Mvc.Data;
using libfintx;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finances.Mvc.Services
{
    public class BankingServices
    {
        private readonly FinTsContextProvider contextProvider;
        private readonly ApplicationDbContext dbContext;

        public BankingServices(FinTsContextProvider contextProvider, ApplicationDbContext dbContext)
        {
            this.contextProvider = contextProvider;
            this.dbContext = dbContext;
        }

        public async Task UpdateAsync(int connectionId)
        {
            var lastSync = await GetLastSync(connectionId);

            var context = await contextProvider.GetContextAsync(connectionId);

            var init = new MyTransactionInit(context, false);
            var balance = new MyTransactionBalance(context);
            var camt = new MyTransactionCamt(context, camtVersion.camt052, lastSync, DateTime.Now);
            var comp = new CompositeTransaction(init, balance, camt);

            context.Transaction = comp;

            await CompleteTransactionAsync(connectionId, null);
        }

        private async Task<DateTime> GetLastSync(int connectionId)
        {
            var data = await dbContext.ConnectionData.FindAsync(connectionId);
            var lastSync = data.LastSync?.AddDays(-1) ?? new DateTime(DateTime.Today.Year, 1, 1);
            return lastSync;
        }

        private async Task TransactionComplete(int connectionId, HBCIDialogResult e)
        {
            switch (e)
            {
                case HBCIDialogResult<List<TStatement>> statements:
                    await CompleteSync(connectionId, statements);
                    break;
                case HBCIDialogResult<AccountBalance> balance:
                    await CompleteBalance(connectionId, balance);
                    break;
            }

        }

        private async Task CompleteBalance(int connectionId, HBCIDialogResult<AccountBalance> balance)
        {
            var data = await dbContext.ConnectionData.FindAsync(connectionId);
            data.LastSync = DateTime.Now;
            data.Balance = (int)balance.Data.Balance;
            await dbContext.SaveChangesAsync();
        }

        private async Task CompleteSync(int connectionId, HBCIDialogResult<List<TStatement>> statements)
        {
            var accountTransactions = statements.Data
                                .SelectMany(x => x.transactions)
                                .Select(x => new AccountItem(x, connectionId))
                                .ToArray();

            var ids = accountTransactions.Select(x => x.Id);
            var present = await dbContext.AccountItems
                .Where(x => ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToArrayAsync();

            accountTransactions = accountTransactions
                .Where(x => !present.Contains(x.Id))
                .ToArray();

            dbContext.AccountItems.AddRange(accountTransactions);
            await dbContext.SaveChangesAsync();
        }

        public async Task CompleteTransactionAsync(int connectionId, string tan = null)
        {
            var context = await contextProvider.GetContextAsync(connectionId);
                        
            await context.Transaction.ContinueAsync(tan);

            while(context.Transaction.TryDequeue(out var result))
            {
                await TransactionComplete(connectionId, result);
            }

            if (context.Transaction.State == TransactionState.Fininshed)
            {
                context.Transaction = null;
            }
        }
    }
}
