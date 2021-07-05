using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWE_Projekt.Models;
using betriebsmittelverwaltung.Data;

namespace betriebsmittelverwaltung.Controllers
{
    public class ResourceHistoriesController : Controller
    {
        private readonly AppDBContext _context;

        public ResourceHistoriesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ResourceHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.RessourceHistories.ToListAsync());
        }

        // GET: ResourceHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceHistory = await _context.RessourceHistories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceHistory == null)
            {
                return NotFound();
            }

            return View(resourceHistory);
        }

        // GET: ResourceHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResourceHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TimeStamp,HiType")] ResourceHistory resourceHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resourceHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resourceHistory);
        }

        // GET: ResourceHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceHistory = await _context.RessourceHistories.FindAsync(id);
            if (resourceHistory == null)
            {
                return NotFound();
            }
            return View(resourceHistory);
        }

        // POST: ResourceHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TimeStamp,HiType")] ResourceHistory resourceHistory)
        {
            if (id != resourceHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resourceHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceHistoryExists(resourceHistory.Id))
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
            return View(resourceHistory);
        }

        // GET: ResourceHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceHistory = await _context.RessourceHistories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resourceHistory == null)
            {
                return NotFound();
            }

            return View(resourceHistory);
        }

        // POST: ResourceHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resourceHistory = await _context.RessourceHistories.FindAsync(id);
            _context.RessourceHistories.Remove(resourceHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceHistoryExists(int id)
        {
            return _context.RessourceHistories.Any(e => e.Id == id);
        }
    }
}
