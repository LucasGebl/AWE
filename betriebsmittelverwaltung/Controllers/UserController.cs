using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWE_Projekt.Models;
using betriebsmittelverwaltung.Data;
using betriebsmittelverwaltung.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace betriebsmittelverwaltung.Controllers
{
    public class UserController : Controller
    {
        private UserManager<User> _userManager;
        public enum SortCriteria
        {
            [Display(Name = "Vorname")]
            ForeName,
            [Display(Name = "Nachname")]
            LastName,
            [Display(Name = "E-Mail")]
            Email
        }

        private readonly AppDBContext _context;

        public UserController(AppDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, [Bind("Id,Email")] User user, string forename, string lastname, string password, string role)
        {
            if (ModelState.IsValid)
            {
                User u = await _userManager.FindByEmailAsync(user.Email);
                if (u == null)
                {
                    u = new User { Email = user.Email, UserName = user.Email, ForeName = forename, LastName = lastname };
                    await _userManager.CreateAsync(u, password);
                    await _userManager.AddToRoleAsync(u, role);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("emailAlreadyUsed", "Die angegebene Email existiert bereits in der Datenbank");
                    return View(user);
                }

              //  await _context.SaveChangesAsync();
                
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,CheckOut")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
