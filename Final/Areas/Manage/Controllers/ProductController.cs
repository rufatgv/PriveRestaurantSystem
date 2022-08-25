using Final.DAL;
using Final.Extensions;
using Final.Helpers;
using Final.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Product> products = await _context.Products
                    .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                      .Include(t => t.Category)


                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);

            return View(products.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Create(bool? status, int page = 1)
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, bool? status, int page = 1)
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Choose the right category");
                return View();
            }
            //if (product.Name.CheckString())
            //{
            //    ModelState.AddModelError("Name", "Name may contain only letters");
            //    return View();
            //}
            if (product.TagIds.Count > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int item in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.Id != item && !t.IsDeleted))
                    {
                        ModelState.AddModelError("TagIds", $"The selected Id {item}  Tag is wrong");
                        return View();
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = item
                    };

                    productTags.Add(productTag);
                }

                product.ProductTags = productTags;
            }
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!product.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                product.Image = product.ImageFile.CreateFile(_env, "assets", "img", "meals");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image must be selected");
                return View();
            }
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public IActionResult Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = _context.Products
                .Include(p => p.Category)
               .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

            Product product = await _context.Products.Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, bool? status, int page = 1)
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();
            Product dbProduct = await _context.Products
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (!ModelState.IsValid)
            {
                return View();
            }



            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Choose the right category");
                return View();
            }

            if (product.TagIds.Count > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int item in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.Id != item && !t.IsDeleted))
                    {
                        ModelState.AddModelError("TagIds", $"The selected Id {item}  Tag is wrong");
                        return View();
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = item
                    };

                    productTags.Add(productTag);
                }

                product.ProductTags = productTags;
            }

            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "The selected image type doesn't match");
                    return View();
                }

                if (!product.ImageFile.CheckFileSize(100000))
                {
                    ModelState.AddModelError("ImageFile", "The Size of the Selected Image Can Be Maximum 10000 Kb");
                    return View();
                }

                Helper.DeleteFile(_env, dbProduct.Image, "assets", "img", "meals");
                dbProduct.Image = product.ImageFile.CreateFile(_env, "assets", "img", "meals");

            }
            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.ProductTags = product.ProductTags;
            dbProduct.TagIds = product.TagIds;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _context.Products.FirstOrDefaultAsync(t => t.Id == id);

            if (dbProduct == null) return NotFound();

            dbProduct.IsDeleted = true;
            dbProduct.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Product> products = await _context.Products

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);



            return PartialView("_ProductIndexPartial", products.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _context.Products.FirstOrDefaultAsync(t => t.Id == id);

            if (dbProduct == null) return NotFound();

            dbProduct.IsDeleted = false;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Product> products = await _context.Products

                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);



            return PartialView("_ProdcutIndexPartial", products.Skip((page - 1) * 5).Take(5));

        }

    }
}
