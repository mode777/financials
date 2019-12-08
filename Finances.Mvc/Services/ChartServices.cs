using Finances.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finances.Mvc.Services
{
    public class ChartServices
    {
        private readonly ApplicationDbContext db;

        public ChartServices(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<(string, double)[]> GroupByTypes(string userId)
        {
            var data = await db.AccountItems
                .Where(x => x.Connection.Owner.Id == userId)
                .GroupBy(x => x.Type)
                .Select(x => new { x.Key, Sum = x.Sum(y => y.Amount) })
                .ToListAsync();

            var tuples = data
                .Select(x => (x.Key, (double)x.Sum))
                .ToArray();

            return tuples;
        }

        public async Task<object> BalanceOverTime(int connectionId)
        {
            var results = new List<(DateTime, int)>();

            var data = await db.ConnectionData.FindAsync(connectionId);
            var amount = data.Balance ?? 0;
            var date = DateTime.Today.AddDays(1);

            var query = db.AccountItems
                .Where(x => x.Connection.Id == connectionId)
                .GroupBy(x => x.ValueDate.Value.Date)
                .OrderByDescending(x => x.Key)
                .Select(x => new { x.Key, Sum = x.Sum(y => y.Amount) });

            results.Add((date, amount));

            foreach (var item in query)
            {
                while(item.Key < date)
                {
                    results.Add((date, amount));
                    date = date.AddDays(-1);
                }

                amount -= (int)(item.Sum * 10);
                results.Add((date, amount));
                date = date.AddDays(-1);
            }

            return results;
        }

    }
}
