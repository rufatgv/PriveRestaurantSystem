using Final.DAL;
using Final.Models;
using Final.ViewModels.Blog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index(int? cid, int? tid, int page = 1)
        {
            ViewBag.cid = cid;
            ViewBag.tid = tid;

            IQueryable<Blog> blogs = _context.Blogs.Where(b => !b.IsDeleted);

            if (cid != null)
            {
                blogs = blogs.Where(b => b.CategoryId == cid);

            }
            if (tid != null)
            {
                blogs = blogs
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                 .Where(b => b.BlogTags.Any(t => t.Tag.Id == tid));
            }

            BlogVM blogVM = new BlogVM
            {
                Blogs = blogs.Skip((page - 1) * 6).Take(3).ToList(),
                Categories = await _context.Categories.Include(c => c.Blogs).Where(c => !c.IsDeleted).Take(12).ToListAsync(),
                Tags = await _context.Tags.Where(c => !c.IsDeleted).Take(12).ToListAsync()
            };
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)blogs.Count() / 6);
            return View(blogVM);
        }

        public async Task<IActionResult> Detail(int? bid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            ViewBag.Blogs = await _context.Blogs.OrderByDescending(b => b.CreatedAt).Take(4).ToListAsync();

            if (bid == null) return BadRequest();
            Blog blog = await _context.Blogs
                 .Include(b => b.Reviews)
                .FirstOrDefaultAsync(p => p.Id == (int)bid);
            if (blog == null) return NotFound();

            BlogVM blogVM = new BlogVM()
            {
                Blog=blog,
                Reviews = await _context.Reviews.Where(b=>b.BlogId==blog.Id).OrderByDescending(b=>b.CreatedAt).ToListAsync()


            };
            return View(blogVM);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int? bid, [FromBody] string message)
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return PartialView("_LoginPartial");
            //}
            if (!User.Identity.IsAuthenticated)
            {
                return Json(0);
            }

            if (bid == null) return View();
            Review review = new Review();
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);
            review.Email = appUser.Email;
            review.Name = appUser.UserName;

            BlogVM blogVM = new BlogVM()
            {
                Blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == bid),
                Blogs = await _context.Blogs.ToListAsync(),
                Reviews = await _context.Reviews
               .Where(p => p.BlogId == bid)
               .OrderByDescending(r => r.CreatedAt)
               .ToListAsync()
            };

            if (message == null || message == "")
            {
                return PartialView("_EditCommentPartial", blogVM);
            }

            review.Message = message.Trim();
            if (review.Star < 0 || review.Star > 5)
            {
                review.Star = 1;
            }

            review.BlogId = (int)bid;
            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            blogVM = new BlogVM()
            {
                Blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == bid),
                Blogs = await _context.Blogs.ToListAsync(),
                Reviews = await _context.Reviews
                .Where(p => p.BlogId == bid)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync()
            };
            return PartialView("_EditCommentPartial", blogVM);
        }

  
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Blog");
            }
            List<Blog> blogs = await _context.Blogs.Where(p => p.Title.ToLower().Contains(query.ToLower())).ToListAsync();
            return View(blogs);
        }
        public async Task<IActionResult> SearchPartial(string query)
        {
            List<Blog> blogs = await _context.Blogs.Where(p => p.Title.ToLower().Contains(query.ToLower())).ToListAsync();
            return PartialView("_BlogSearchPartial", blogs);
        }
        public async Task<IActionResult> CategoryFilter(int? id)
        {
            List<Blog> blog = await _context.Blogs.Where(p => p.CategoryId == id && !p.IsDeleted).Take(6).ToListAsync();

            return PartialView("_BlogCategoryPartial", blog);
        }
        public async Task<IActionResult> TagFilter(int? id)
        {
            List<Blog> blog = await _context.Blogs.Include(p => p.BlogTags).ThenInclude(pt => pt.Tag)
                 .Where(p => p.BlogTags.Any(t => t.Tag.Id == id)).Take(6).ToListAsync();
            return PartialView("_BlogCategoryPartial", blog);
        }
    }
} 
