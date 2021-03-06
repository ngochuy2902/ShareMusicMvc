using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareMusic.Mvc.Data;
using ShareMusic.Mvc.ViewModels;

namespace ShareMusic.Mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ShareMusicMvcUser> _userManager;
        private readonly ShareMusicMvcContext _context;

        public AdminController(ShareMusicMvcContext context, UserManager<ShareMusicMvcUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            //var users = await _userManager.Users.ToListAsync();
            var users = from user in _userManager.Users select user;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(user => user.UserName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(user => user.UserName);
                    break;
                case "Date":
                    users = users.OrderBy(users => users.JoinTime);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(users => users.JoinTime);
                    break;
                default:
                    users = users.OrderBy(user => user.UserName);
                    break;
            }
            return View(await users.AsNoTracking().ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            var posts = await _context.Posts.Include(c=>c.Category).Where(p => p.UserId == id).ToListAsync();
            UserViewModels userViewModel = new UserViewModels()
            {
                Username = currentUser.UserName,
                Phone = currentUser.PhoneNumber,
                JoinTime = currentUser.JoinTime,
                Posts = posts
            };
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userToDelete = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (userToDelete == null)
            {
                return NotFound();
            }

            return View(userToDelete);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(userToDelete);
            return RedirectToAction("Index");
        }
    }
}