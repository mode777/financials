using Finances.Mvc.Data;
using libfintx;
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
        private readonly IServiceProvider serviceProvider;

        public BankingServices(FinTsContextProvider contextProvider, IServiceProvider serviceProvider)
        {
            this.contextProvider = contextProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task<HBCIDialogResult> SyncAccountAsync(int connectionId, DateTime start, DateTime end)
        {
            var context = await contextProvider.GetContextAsync(connectionId);

            var init = new MyTransactionInit(context, false);
            var camt = new MyTransactionCamt(context, camtVersion.camt052, start, end);
            var comp = new CompositeTransaction(init, camt);

            context.Transaction = comp;
         
            return await CompleteTransactionAsync(connectionId, null);
        }

        public async Task<HBCIDialogResult> SyncBalanceAsync(int connectionId)
        {
            var context = await contextProvider.GetContextAsync(connectionId);

            var init = new MyTransactionInit(context, false);
            var camt = new MyTransactionBalance(context);
            var comp = new CompositeTransaction(init, camt);

            context.Transaction = comp;

            return await CompleteTransactionAsync(connectionId, null);
        }


        private void SyncComplete(int connectionId, HBCIDialogResult e)
        {
            if(e is HBCIDialogResult<List<TStatement>> statements)
            {
                var accountTransactions = statements.Data
                    .SelectMany(x => x.transactions)
                    .Select(x => new AccountItem(x, connectionId))
                    .ToArray();
                    
                using (var scope = serviceProvider.CreateScope())
                using (var db = scope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    var ids = accountTransactions.Select(x => x.Id);
                    var present = db.AccountItems.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToArray();

                    accountTransactions = accountTransactions.Where(x => !present.Contains(x.Id)).ToArray();

                    db.AccountItems.AddRange(accountTransactions);
                    db.SaveChanges();
                }
            }
            else if(e is HBCIDialogResult<AccountBalance> balance)
            {

            }
        }

        public async Task<HBCIDialogResult> CompleteTransactionAsync(int connectionId, string tan = null)
        {
            var context = await contextProvider.GetContextAsync(connectionId);
                        
            await context.Transaction.ContinueAsync(tan);
            var result = context.Transaction.Result;

            if (context.Transaction.State == TransactionState.Fininshed)
            {
                SyncComplete(connectionId, result);
                context.Transaction = null;
            }

            return result;
        }
    }
}
