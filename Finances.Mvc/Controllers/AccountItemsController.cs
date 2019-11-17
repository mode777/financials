using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finances.Mvc.Data;

namespace Finances.Mvc.Controllers
{
    public class AccountItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AccountItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AccountItems.Include(a => a.Connection);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AccountItems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountItem = await _context.AccountItems
                .Include(a => a.Connection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accountItem == null)
            {
                return NotFound();
            }

            return View(accountItem);
        }

        // GET: AccountItems/Create
        public IActionResult Create()
        {
            ViewData["ConnectionId"] = new SelectList(_context.ConnectionData, "Id", "AccountId");
            return View();
        }

        // POST: AccountItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountCode,Amount,BankCode,Description,InputDate,MandateId,PartnerName,Type,ValueDate,ConnectionId")] AccountItem accountItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accountItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConnectionId"] = new SelectList(_context.ConnectionData, "Id", "AccountId", accountItem.ConnectionId);
            return View(accountItem);
        }

        // GET: AccountItems/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountItem = await _context.AccountItems.FindAsync(id);
            if (accountItem == null)
            {
                return NotFound();
            }
            ViewData["ConnectionId"] = new SelectList(_context.ConnectionData, "Id", "AccountId", accountItem.ConnectionId);
            return View(accountItem);
        }

        // POST: AccountItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,AccountCode,Amount,BankCode,Description,InputDate,MandateId,PartnerName,Type,ValueDate,ConnectionId")] AccountItem accountItem)
        {
            if (id != accountItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accountItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountItemExists(accountItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConnectionId"] = new SelectList(_context.ConnectionData, "Id", "AccountId", accountItem.ConnectionId);
            return View(accountItem);
        }

        // GET: AccountItems/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountItem = await _context.AccountItems
                .Include(a => a.Connection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accountItem == null)
            {
                return NotFound();
            }

            return View(accountItem);
        }

        // POST: AccountItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var accountItem = await _context.AccountItems.FindAsync(id);
            _context.AccountItems.Remove(accountItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountItemExists(string id)
        {
            return _context.AccountItems.Any(e => e.Id == id);
        }
    }
}
