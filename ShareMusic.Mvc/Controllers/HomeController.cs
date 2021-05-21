using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShareMusic.Mvc.Data;
using ShareMusic.Mvc.Models;

namespace ShareMusic.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShareMusicMvcContext _context;
        public HomeController(ILogger<HomeController> logger, ShareMusicMvcContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.Include(x => x.Category).ToListAsync());
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Album()
        {
            return View();
        }

        public PartialViewResult FilterIndex(IFormCollection collection)
        {
            var filteredPosts = _context.Posts.Include(x => x.Category).Select(p => p);
            var duration = collection.ContainsKey("duration") == false ? "all" : collection["duration"][0];
            switch (duration)
            {
                case "today":
                    filteredPosts = filteredPosts.Where(p => p.PostTime.Date == DateTime.Now.Date);
                    break;
                case "3days":
                    filteredPosts = filteredPosts.Where(p => p.PostTime.Date.AddDays(3) >= DateTime.Now.Date);
                    break;
                case "week":
                    filteredPosts = filteredPosts.Where(p => p.PostTime.Date.AddDays(7) >= DateTime.Now.Date);
                    break;
                default:
                    break;
            }
            return PartialView("NewsFeedPartial", filteredPosts.ToList());
        }
    }
}