using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShareMusic.Mvc.Data;
using ShareMusic.Mvc.Models;
using ShareMusic.Mvc.ViewModels;

namespace ShareMusic.Mvc.Controllers
{
    public class PostsController : Controller
    {
        private readonly ShareMusicMvcContext _context;
        //private readonly UserManager<ShareMusicMvcUser> _userManager;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostsController(ShareMusicMvcContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            //_userManager = userManager;
        }

        // GET: Posts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.Include(c=>c.Category).ToListAsync());
        }

        public async Task<IActionResult> YourPosts()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Posts.Include(x=>x.Category).Where(post => post.UserId == CurrentUserId).ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(x => x.Category).FirstOrDefaultAsync(m => m.Id == id);
            var comments = await _context.Comments.Where(comment => comment.PostId == id).ToListAsync();
            var musics = await _context.Musics.Where(music => music.PostId == id).ToArrayAsync();
            CommentViewModels commentViewModels = new CommentViewModels()
            {
                Post = post,
                Comments = comments,
                Musics = musics
            };
            if (post == null)
            {
                return NotFound();
            }

            return View(commentViewModels);
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CategoryId,Description,PostTime,MusicPaths,ImagePath")] PostViewModels model)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = currentUserId;

            if (ModelState.IsValid)
            {
                string fileName = null;
                if (model.ImagePath != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    fileName = Guid.NewGuid().ToString() + "-" + model.ImagePath.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    model.ImagePath.CopyTo(new FileStream(filePath, FileMode.Create));    
                }
                Post post = new Post()
                {
                    Title = model.Title,
                    Description = model.Description,
                    PostTime = model.PostTime,
                    CategoryId = model.CategoryId,
                    UserId = model.UserId,
                    ImagePath = fileName

                };
                _context.Add(post);
                await _context.SaveChangesAsync();
                fileName = null;
                if (model.MusicPaths != null && model.MusicPaths.Count > 0)
                {
                    foreach (IFormFile music in model.MusicPaths)
                    {
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "musics");
                        fileName = Guid.NewGuid().ToString() + "-" + music.FileName;
                        string filePath = Path.Combine(uploadDir, fileName);
                        music.CopyTo(new FileStream(filePath, FileMode.Create));
                        Music ms = new Music()
                        {
                            PostId = post.Id,
                            MusicName = music.FileName.Replace(".mp3",""),
                            MusicURL = fileName,
                        };
                        _context.Add(ms);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(YourPosts));
            }
            return View();
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CategoryId,ImagePath")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currenUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var category = _context.Categories.Where(c => c.CategoryId == post.CategoryId).FirstOrDefault();
                    post.UserId = currenUserId;
                    post.Category = category;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(YourPosts));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(c=>c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(YourPosts));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        public PartialViewResult AddComment(IFormCollection collection)
        {
            int postId = Convert.ToInt32(collection["postId"][0]);
            var currentPost = _context.Posts.Where(p => p.Id == postId).FirstOrDefault();
            string userId = collection["userId"][0];
            Comment newComment = new Comment()
            {
                Content = collection["comment"][0],
                UserId = userId,
                PostId = currentPost.Id,
                Post = currentPost
            };
            _context.Add(newComment);
            _context.SaveChanges();

            CommentViewModels commentViewModel = new CommentViewModels()
            {
                Post = currentPost,
                Comments = currentPost.Comments



            };

            return PartialView("CommentPartial", commentViewModel);
        }
        public async Task<IActionResult> MusicAlbum(int id)
        {
            return View(await _context.Posts.Include(c => c.Category).Where(x => x.CategoryId == id).ToArrayAsync());
        }
    }
}
