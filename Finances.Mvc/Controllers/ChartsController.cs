using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finances.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Finances.Mvc.Controllers
{
    [Authorize]
    public class ChartsController : Controller
    {
        private readonly UserManager<IdentityUser> users;
        private readonly ChartServices services;

        public ChartsController(UserManager<IdentityUser> users, ChartServices services)
        {
            this.users = users;
            this.services = services;
        }

        public IActionResult Index()
        {
            return View();
        }

        private string UserId => users.GetUserId(User);

        [HttpGet("groupByType")]
        public async Task<IActionResult> GroupByType()
        {
            var data = await services.GroupByTypes(UserId);

            return Json(new 
            {
                Data = data.Select(x => x.Item2),
                Labels = data.Select(x => x.Item1)
            });
        }
    }
}