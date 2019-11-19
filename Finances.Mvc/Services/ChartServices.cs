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
    }
}
