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
using Microsoft.AspNetCore.Identity;

namespace betriebsmittelverwaltung.Controllers
{
    public class ReturnsController : Controller
    {

        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            Resource,
            CheckIn,
            ReturnStatus,
            Creator

        }

        private readonly AppDBContext _context;
        private readonly UserManager<Areas.Identity.Data.User> _userManager;

        public ReturnsController(AppDBContext context, UserManager<Areas.Identity.Data.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Lagerist")]
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

            return View(await query.Skip(PageSize * (Page - 1)).Take(PageSize).Include(x => x.Creator).Include(x => x.Resource).ToListAsync());
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @return = await _context.Returns
                .Include(x => x.Resource)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@return == null)
            {
                return NotFound();
            }

            return View(@return);
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Return @return = new Return { ReturnStatus = ReturnStatus.unbestätigt };
            if (ModelState.IsValid)
            {

                @return.Resource = await _context.Resources.Where(x => x.Id == id).FirstOrDefaultAsync();
                @return.Creator = await _userManager.GetUserAsync(User);
                _context.Add(@return);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@return);

            //ViewData["Resources"] = _context.Resources.Where(x => x.ConstructionSite != null).ToList();
            //return View();
        }

        // POST: Returns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Create([Bind("Id,CheckIn")] Return @return, int resourceId)
        {
            if (ModelState.IsValid)
            {
                @return.Resource = await _context.Resources.Where(x => x.Id == resourceId).FirstOrDefaultAsync();
                @return.Creator = await _userManager.GetUserAsync(User);
                _context.Add(@return);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@return);
        }

        [Authorize(Roles = "Admin,Lagerist")]
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
        [Authorize(Roles = "Admin,Lagerist")]
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

        public async Task<IActionResult> Confirm(int? id, ConstructionSite constructionSite)
        {
            if (id == null || constructionSite == null)
            {
                return NotFound();
            }

            var @return = await _context.Returns.Where(x => x.Id == id).Include(x => x.Creator).Include(x => x.Resource).FirstOrDefaultAsync();
            if (@return == null)
            {
                return NotFound();
            }

            try
            {
                @return.ReturnStatus = ReturnStatus.bestätigt;
                @return.CheckIn = DateTime.Now;
                if (@return.Resource != null)
                {
                    var result = _context.Resources.SingleOrDefault(b => b.Id == @return.Resource.Id);
                    if (result != null)
                    {
                        result.ConstructionSite = null;
                        result.Available = true;
                        _context.SaveChanges();
                    }

                }

                _context.Update<Resource>(@return.Resource);
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
            // return View(@return);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Confirm(int id, [Bind("Id")] Return @return)
        {
            if (id != @return.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @return.ReturnStatus = ReturnStatus.bestätigt;
                    @return.CheckIn = DateTime.Now;
                    @return.Resource.ConstructionSite = null;
                    @return.Resource.Available = true;

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
                return View(@return);
                //return RedirectToAction(nameof(Index));
            }
            return View(@return);
        }

        // GET: Returns/Delete/5
        [Authorize(Roles = "Admin,Lagerist")]
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
        [Authorize(Roles = "Admin,Lagerist")]
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
