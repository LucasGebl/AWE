using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWE_Projekt.Models;
using betriebsmittelverwaltung.Data;
using System.ComponentModel.DataAnnotations;

namespace betriebsmittelverwaltung.Controllers
{
    public class ReturnsController : Controller
    {

        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            Resource,
            [Display(Name = "Construction Site")]
            ConstructionSite,
            Manager
        }

        private readonly AppDBContext _context;

        public ReturnsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Returns
        public async Task<IActionResult> Index()
        {
            return View(await _context.Returns.ToListAsync());
        }

        // GET: Returns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @return = await _context.Returns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@return == null)
            {
                return NotFound();
            }

            return View(@return);
        }

        // GET: Returns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Returns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CheckIn")] Return @return)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@return);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@return);
        }

        // GET: Returns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @return = await _context.Returns.FindAsync(id);
            if (@return == null)
            {
                return NotFound();
            }
            return View(@return);
        }

        // POST: Returns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CheckIn")] Return @return)
        {
            if (id != @return.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@return);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnExists(@return.Id))
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
            return View(@return);
        }

        // GET: Returns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @return = await _context.Returns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@return == null)
            {
                return NotFound();
            }

            return View(@return);
        }

        // POST: Returns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @return = await _context.Returns.FindAsync(id);
            _context.Returns.Remove(@return);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnExists(int id)
        {
            return _context.Returns.Any(e => e.Id == id);
        }
    }
}
