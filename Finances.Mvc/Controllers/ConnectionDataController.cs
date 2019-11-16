using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finances.Mvc.Data;
using Microsoft.AspNetCore.Identity;

namespace Finances.Mvc.Controllers
{
    public class ConnectionDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> users;

        public ConnectionDataController(ApplicationDbContext context, UserManager<IdentityUser> users)
        {
            _context = context;
            this.users = users;
        }

        // GET: ConnectionData
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConnectionData.ToListAsync());
        }

        // GET: ConnectionData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connectionData = await _context.ConnectionData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (connectionData == null)
            {
                return NotFound();
            }

            return View(connectionData);
        }

        // GET: ConnectionData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConnectionData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Blz,AccountId,Iban,FinTsServer,UserId,Pin")] ConnectionData connectionData)
        {
            connectionData.Owner = await users.FindByIdAsync(users.GetUserId(User));
            TryValidateModel(connectionData);

            //if (ModelState.IsValid)
            //{
                _context.Add(connectionData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            return View(connectionData);
        }

        // GET: ConnectionData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connectionData = await _context.ConnectionData.FindAsync(id);
            if (connectionData == null)
            {
                return NotFound();
            }
            return View(connectionData);
        }

        // POST: ConnectionData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Blz,AccountId,Iban,FinTsServer,UserId,Pin")] ConnectionData connectionData)
        {
            if (id != connectionData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(connectionData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConnectionDataExists(connectionData.Id))
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
            return View(connectionData);
        }

        // GET: ConnectionData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var connectionData = await _context.ConnectionData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (connectionData == null)
            {
                return NotFound();
            }

            return View(connectionData);
        }

        // POST: ConnectionData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var connectionData = await _context.ConnectionData.FindAsync(id);
            _context.ConnectionData.Remove(connectionData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConnectionDataExists(int id)
        {
            return _context.ConnectionData.Any(e => e.Id == id);
        }
    }
}
