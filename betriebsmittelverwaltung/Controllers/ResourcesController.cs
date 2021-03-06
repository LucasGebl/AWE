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
    public class ResourcesController : Controller
    {
        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            Name,
            [Display(Name = "Kaufdatum")]
            BuyDate,
            [Display(Name = "Typ")]
            Type,
            [Display(Name = "Wartungsintervall")]
            MaintenanceInterval,
            [Display(Name = "Nutzungsrate")]
            UtilizationRate,
            [Display(Name = "Baustelle")]
            ConstructionSite,
        }

        private readonly AppDBContext _context;

        public ResourcesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Resources
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Index(string Search, string Filter, SortCriteria Sort = SortCriteria.Id, int Page = 1, int PageSize = 10)
        {
            IQueryable<Resource> query = _context.Resources;
            query = (Search != null) ? query.Where(m => (m.Name.Contains(Search))) : query;
            // query = (Filter != null) ? query.Where(m => (m.Manufacturer == Filter)) : query;

            switch (Sort)
            {
                case SortCriteria.Id:
                    query = query.OrderBy(m => m.Id);
                    break;
                case SortCriteria.Name:
                    query = query.OrderBy(m => m.Name);
                    break;
                case SortCriteria.BuyDate:
                    query = query.OrderBy(m => m.BuyDate);
                    break;
                case SortCriteria.Type:
                    query = query.OrderBy(m => m.Type);
                    break;
                case SortCriteria.MaintenanceInterval:
                    query = query.OrderBy(m => m.MaintenanceInterval);
                    break;
                case SortCriteria.UtilizationRate:
                    query = query.OrderBy(m => m.UtilizationRate);
                    break;
                case SortCriteria.ConstructionSite:
                    query = query.OrderBy(m => m.ConstructionSite);
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

            var resHis = await _context.RessourceHistories.Include(x => x.Resource).ToListAsync();
            foreach (var id in resHis)
            {
                var resultTime = Divide(id.Span, (ulong)((DateTime.Now - id.Resource.BuyDate).TotalSeconds));
                ViewData["ResourceHistory_" + id.Id] = resultTime;
            }


            return View(await query.Skip(PageSize * (Page - 1)).Take(PageSize).Include(x => x.ConstructionSite).ToListAsync());
        }

        public static double Divide(UInt64 dividend, UInt64 divisor)
        {
            return (double)dividend / (double)divisor;
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(x => x.ConstructionSite)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Create([Bind("Id,Name,BuyDate,Type,UtilizationRate,MaintenanceInterval")] Resource resource)
        {
            if (ModelState.IsValid)
            {
                resource.Available = true;
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Resources/Edit/5
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BuyDate,Type,UtilizationRate,MaintenanceInterval,Available")] Resource resource)
        {
            if (id != resource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(resource.Id))
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
            return View(resource);
        }

        // GET: Resources/Delete/5
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(x => x.ConstructionSite)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (resource.ConstructionSite != null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.Where(x => x.Id == id).Include(x => x.ConstructionSite).FirstOrDefaultAsync();

            var orders = await _context.Orders.Where(x => x.Resource == resource).ToListAsync();
            foreach (var item in orders)
            {
                _context.Remove(item);
            }

            var returns = await _context.Returns.Where(x => x.Resource == resource).ToListAsync();
            foreach (var item in returns)
            {
                _context.Remove(item);
            }


            var resHistory = await _context.RessourceHistories.Where(x => x.Resource == resource).ToListAsync();
            foreach (var item in resHistory)
            {
                _context.Remove(item);
            }


            await _context.SaveChangesAsync();


            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.Id == id);
        }
    }
}
