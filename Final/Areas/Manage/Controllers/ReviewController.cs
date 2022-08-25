﻿using Final.DAL;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        public ReviewController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;
            List<Review> reviews = new List<Review>();
            if (status == null)
            {
                reviews = await _context.Reviews
                    .Include(r => r.Blog)
                    .Include(r=>r.Product)
                    .ToListAsync();
            }
            else
            {
                reviews = await _context.Reviews
                    .Include(r => r.Blog)
                    .Include(r=>r.Product)
                    .Where(r => r.IsDeleted == status)
                    .ToListAsync();

            }
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)reviews.Count() / 5);
            return View(reviews.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Review dbReview = await _context.Reviews
                .Include(r => r.Blog)
                .Include(r=>r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (dbReview == null) return NotFound();

            return View(dbReview);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Review review)
        {
            if (id == null) return BadRequest();
            Review dbReview = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (dbReview == null) return NotFound();

            if (review.Id != id) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Reviews.FirstOrDefault(r => r.Id == id).Id == id);
            Product product = await _context.Products.FirstOrDefaultAsync(b => b.Reviews.FirstOrDefault(r => r.Id == id).Id == id);

            dbReview.Message = review.Message;
            dbReview.UpdatedAt = DateTime.UtcNow.AddHours(4);
            int bid = blog.Id;
            int pid = product.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "review", "manage");
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Review review = await _context.Reviews
                .Include(r => r.Blog)
                     .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();

            return View(review);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Review review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();
            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest();

            Review review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();
            review.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
    }
}
