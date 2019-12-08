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
            var amount = (data.Balance ?? 0) * 10;
            var date = DateTime.Today.AddDays(1);

            var query = db.AccountItems
                .Where(x => x.Connection.Id == connectionId)
                .GroupBy(x => x.InputDate.Value.Year +"-"+ x.InputDate.Value.Month)
                .OrderByDescending(x => x.Key)
                .Select(x => new { x.Key, Sum = x.Sum(y => y.Amount) });

            foreach (var item in query)
            {
                //while(DateTime.Parse(item.Key) < date)
                //{
                //    results.Add((date, amount));
                //    date = date.AddDays(-1);
                //}

                amount -= (int)(item.Sum * 100);
                results.Add((DateTime.Parse(item.Key), amount));
                //date = date.AddDays(-1);
            }

            //results = results.GroupBy(x => new DateTime(x.Item1.Year, x.Item1.Month, 1)).Select(x => (x.Key, x.Sum(y => y.Item2))).ToList();
            

            return new
            {
                Data = results.Select(x => ((double)x.Item2) / 100).Reverse().ToArray(),
                Labels = results.Select(x => x.Item1.ToString("yyyy-MM-dd")).Reverse().ToArray()
            };
        }

    }
}
