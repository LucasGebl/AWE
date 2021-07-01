﻿using System;
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
    public class ConstructionSitesController : Controller
    {
        public enum SortCriteria
        {
            [Display(Name = "ID")]
            Id,
            Name,
            Description,
            Manager
        }

        private readonly AppDBContext _context;

        public ConstructionSitesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ConstructionSites
        public async Task<IActionResult> Index(string Search, string Filter, SortCriteria Sort = SortCriteria.Id, int Page = 1, int PageSize = 10)
        {
            IQueryable<ConstructionSite> query = _context.ConstructionSites;
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
                case SortCriteria.Description:
                    query = query.OrderBy(m => m.Description);
                    break;
                case SortCriteria.Manager:
                    query = query.OrderBy(m => m.Manager);
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

        // GET: ConstructionSites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionSite = await _context.ConstructionSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (constructionSite == null)
            {
                return NotFound();
            }

            return View(constructionSite);
        }

        // GET: ConstructionSites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConstructionSites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ConstructionSite constructionSite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(constructionSite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(constructionSite);
        }

        // GET: ConstructionSites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionSite = await _context.ConstructionSites.FindAsync(id);
            if (constructionSite == null)
            {
                return NotFound();
            }
            return View(constructionSite);
        }

        // POST: ConstructionSites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ConstructionSite constructionSite)
        {
            if (id != constructionSite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(constructionSite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConstructionSiteExists(constructionSite.Id))
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
            return View(constructionSite);
        }

        // GET: ConstructionSites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionSite = await _context.ConstructionSites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (constructionSite == null)
            {
                return NotFound();
            }

            return View(constructionSite);
        }

        // POST: ConstructionSites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var constructionSite = await _context.ConstructionSites.FindAsync(id);
            _context.ConstructionSites.Remove(constructionSite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConstructionSiteExists(int id)
        {
            return _context.ConstructionSites.Any(e => e.Id == id);
        }
    }
}