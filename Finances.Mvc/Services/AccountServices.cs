using Dapper;
using Finances.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Finances.Mvc.Services
{
    public class AccountServices
    {
        private readonly IDbConnection db;

        public AccountServices(ApplicationDbContext db)
        {
            this.db = db.Database.GetDbConnection();
        }

        public string[] GetCategories()
        {
            db.QueryAsync<string[]>("select name from AccountItems");
        }


    }
}
