using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Finances.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Finances.Mvc.Services;

namespace Finances.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AccountServices accounts;

        public HomeController(AccountServices accounts)
        {
            this.accounts = accounts;
        }

        public async Task<IActionResult> Index()
        {
            var cat = await accounts.GetCategories();

            return View(cat);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
