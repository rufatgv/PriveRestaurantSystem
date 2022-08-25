using Final.DAL;
using Final.Models;
using Final.ViewModels.Products;
using Final.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string sortby, int? cid, int? tid, int page = 1)
        {
            ViewBag.cid = cid;
            ViewBag.tid = tid;
            IQueryable<Product> products = _context.Products;
            if (cid != null)
            {
                products = products.Where(p => p.CategoryId == cid);

            }
            if (tid != null)
            {
                products = products
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                 .Where(p => p.ProductTags.Any(t => t.Tag.Id == tid));
            }

            switch (sortby)
            {
                case "AZ":
                    products = products.Where(p => !p.IsDeleted).Skip((page - 1) * 6).Take(6).OrderBy(p => p.Name);
                    break;
                case "ZA":
                    products = products.Where(p => !p.IsDeleted).Skip((page - 1) * 6).Take(6).OrderByDescending(p => p.Name);
                    break;
                case "LH":
                    products = products.Where(p => !p.IsDeleted).Skip((page - 1) * 6).Take(6).OrderBy(p => p.Price);
                    break;
                case "HL":
                    products = products.Where(p => !p.IsDeleted).Skip((page - 1) * 6).Take(6).OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.Where(p => !p.IsDeleted).Skip((page - 1) * 6).Take(6).OrderBy(p => p.Name);
                    break;
            }

            ShopVM shopVM = new ShopVM
            {
                Products = products.ToList(),
                Categories = await _context.Categories.Include(c => c.Products).Where(c => !c.IsDeleted).Take(8).ToListAsync(),
                Tags = await _context.Tags.Where(T => !T.IsDeleted).Take(12).ToListAsync()
            };
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)_context.Products.Where(b => !b.IsDeleted).Count() / 6);

            return View(shopVM);
        }

        public async Task<IActionResult> Detail(int? pid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            if (pid == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p=>p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == (int)pid);

            if (product == null) return NotFound();
            ProductVM productVM = new ProductVM()
            {
                Product = product,
                Products = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId)
                .Take(3)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(),
                Reviews = await _context.Reviews.Where(p=>p.ProductId == product.Id).ToListAsync()
            };
            return View(productVM);
        }
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Shop");
            }
            List<Product> products = await _context.Products.Where(p => p.Name.ToLower().Contains(query.ToLower())).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> SearchPartial(string query)
        {
            List<Product> products = await _context.Products.Where(p => p.Name.ToLower().Contains(query.ToLower())).ToListAsync();
            return PartialView("_ProductSearchPartial", products);
        }
        public async Task<IActionResult> CategoryFilter(int? id)
        {
            List<Product> product = await _context.Products.Where(p => p.CategoryId == id && !p.IsDeleted).Take(6).ToListAsync();

            return PartialView("_ShopCategoryPartial", product);
        }
        public async Task<IActionResult> TagFilter(int? id)
        {
            List<Product> product = await _context.Products.Include(p=> p.ProductTags).ThenInclude(pt => pt.Tag)
                 .Where(p => p.ProductTags.Any(t => t.Tag.Id == id)).Take(6).ToListAsync();
            return PartialView("_ShopCategoryPartial", product);
        }
    }
}
