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
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string id, [Bind("Id,Email")] User user, string forename, string lastname, string password, string role)
        {
            if (ModelState.IsValid)
            {
                User u = await _userManager.FindByEmailAsync(user.Email);
                if (u == null)
                {
                    u = new User { Email = user.Email, UserName = user.Email, ForeName = forename, LastName = lastname };
                    var result = await _userManager.CreateAsync(u, password);
                    if (!result.Succeeded)
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(user);
                    }
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, string role, string forename, string lastname, string email, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, role);

                    user.Email = email;
                    user.UserName = email;
                    user.LastName = lastname;
                    user.ForeName = forename;

                    if (!String.IsNullOrWhiteSpace(newPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                        if (!result.Succeeded)
                        {
                            foreach (IdentityError error in result.Errors)
                                ModelState.AddModelError("", error.Description);
                            return View(user);
                        }

                    }

                    await _userManager.UpdateAsync(user);


                    //_context.Update(user);
                    //await _context.SaveChangesAsync();
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

        [Authorize(Roles = "Admin")]
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

        public async Task<bool> IsCurrentUser(User user)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return user.Id == currentUser.Id;

        }
    }
}
