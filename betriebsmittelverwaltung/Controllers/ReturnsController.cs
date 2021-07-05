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
using Microsoft.AspNetCore.Authorization;

namespace betriebsmittelverwaltung.Controllers
{
    public class ReturnsController : Controller
    {

        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            [Display(Name = "Ressource")]
            Resource,
            [Display(Name = "Check In")]
            CheckIn,
            [Display(Name = "Retourstatus")]
            ReturnStatus,
            [Display(Name = "Ersteller")]
            Creator

        }

        private readonly AppDBContext _context;

        public ReturnsController(AppDBContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
        public async Task<IActionResult> Index(string Search, string Filter, SortCriteria Sort = SortCriteria.Id, int Page = 1, int PageSize = 10)
        {
            IQueryable<Return> query = _context.Returns;
            //query = (Search != null) ? query.Where(m => (m.Name.Contains(Search))) : query;
            // query = (Filter != null) ? query.Where(m => (m.Manufacturer == Filter)) : query;

            switch (Sort)
            {
                case SortCriteria.Id:
                    query = query.OrderBy(m => m.Id);
                    break;
                case SortCriteria.Resource:
                    query = query.OrderBy(m => m.Resource);
                    break;
                case SortCriteria.CheckIn:
                    query = query.OrderBy(m => m.CheckIn);
                    break;
                case SortCriteria.ReturnStatus:
                    query = query.OrderBy(m => m.ReturnStatus);
                    break;
                case SortCriteria.Creator:
                    query = query.OrderBy(m => m.Creator);
                    break;
                    break;
                default:
                    query = query.OrderBy(m => m.Id);
                    break;
            }

            int PageTotal = ((await query.CountAsync()) + PageSize - 1) / PageSize;
            Page = (Page > PageTotal) ? PageTotal : Page;
            Page = (Page < 1) ? 1 : Page;

            ViewBag.Search = Search;
            //   ViewBag.Filter = Filter;
            //   ViewBag.FilterValues = new SelectList(await _context.ConstructionSites.Select(m => m.Manufacturer).Distinct().ToListAsync());
            ViewBag.Sort = Sort;
            ViewBag.Page = Page;
            ViewBag.PageTotal = PageTotal;
            ViewBag.PageSize = PageSize;

            return View(await query.Skip(PageSize * (Page - 1)).Take(PageSize).ToListAsync());
        }

        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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

        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Returns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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

        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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
        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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
        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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
        [Authorize(Roles = "Admin,Bauleiter,Lagerist")]
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
