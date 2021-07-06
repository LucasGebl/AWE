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
using betriebsmittelverwaltung.Areas.Identity.Data;

namespace betriebsmittelverwaltung.Controllers
{
    public class OrdersController : Controller
    {
        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            Resource,
            [Display(Name = "Baustelle")]
            ConstructionSite,
            CheckOut,
            [Display(Name = "Auftragsstatus")]
            OrderStatus,
            [Display(Name = "Ersteller")]
            Creator
        }

        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;
        public OrdersController(AppDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Index(string Search, string Filter, SortCriteria Sort = SortCriteria.Id, int Page = 1, int PageSize = 10)
        {
            IQueryable<Order> query = _context.Orders;
            // query = (Search != null) ? query.Where(m => m.Id.Contains(Search)) : query;
            // query = (Filter != null) ? query.Where(m => (m.Manufacturer == Filter)) : query;

            switch (Sort)
            {
                case SortCriteria.Id:
                    query = query.OrderBy(m => m.Id);
                    break;
                case SortCriteria.Resource:
                    query = query.OrderBy(m => m.Resource);
                    break;
                case SortCriteria.ConstructionSite:
                    query = query.OrderBy(m => m.ConstructionSite);
                    break;
                case SortCriteria.CheckOut:
                    query = query.OrderBy(m => m.CheckOut);
                    break;
                case SortCriteria.OrderStatus:
                    query = query.OrderBy(m => m.OrderStatus);
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

            return View(await query.Skip(PageSize * (Page - 1)).Take(PageSize)
                                .Include(x => x.Resource)
                .Include(x => x.Creator)
                .Include(x => x.ConstructionSite)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(x => x.Resource)
                .Include(x => x.Creator)
                .Include(x => x.ConstructionSite)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create/2
        public IActionResult Create(int? constructionSiteId)
        {
            if(constructionSiteId == null)
            {
                return NotFound();
            }
            ViewData["ConstructionSiteId"] = constructionSiteId;

            ViewData["Resources"] = _context.Resources.Where(x => x.Available == true).ToList();
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Create([Bind("Id,CheckOut")] Order order, int resourceId, int constructionId)
        {
            if (ModelState.IsValid)
            {
      
                order.ConstructionSite = await _context.ConstructionSites.Where(x => x.Id == constructionId).FirstOrDefaultAsync();
                order.Creator = await _userManager.GetUserAsync(User);
                order.Resource = await _context.Resources.Where(x => x.Id == resourceId).FirstOrDefaultAsync();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CheckOut")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Lagerist")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(x => x.Resource).Include(x => x.Creator).Include(x => x.ConstructionSite).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                order.OrderStatus = OrderStatus.Erledigt;
                order.Resource.Available = false;
                order.Resource.ConstructionSite = order.ConstructionSite;
               // order.ConstructionSite.Resources.Add(order.Resource);
                order.CheckOut = DateTime.Now;

                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            // return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
