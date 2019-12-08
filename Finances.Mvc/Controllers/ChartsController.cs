using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finances.Mvc.Data;
using Finances.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finances.Mvc.Controllers
{
    [Authorize]
    public class ChartsController : Controller
    {
        private readonly UserManager<IdentityUser> users;
        private readonly ChartServices services;
        private readonly ApplicationDbContext db;

        public ChartsController(UserManager<IdentityUser> users, ChartServices services, ApplicationDbContext db)
        {
            this.users = users;
            this.services = services;
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        private string UserId => users.GetUserId(User);

        public async Task<IActionResult> GroupByType()
        {
            var data = await services.GroupByTypes(UserId);

            return Json(new 
            {
                Data = data.Select(x => x.Item2),
                Labels = data.Select(x => x.Item1)
            });
        }

        public async Task<IActionResult> BalanceOverTime()
        {
            var cId = await GetConnectionId();
            var data = await services.BalanceOverTime(cId);

            return Json(data);
        }

        private async Task<int> GetConnectionId()
        {
            var userId = users.GetUserId(User);
            var connection = await db.ConnectionData.FirstOrDefaultAsync(x => x.Owner.Id == userId);
            return connection.Id;
        }


    }
}