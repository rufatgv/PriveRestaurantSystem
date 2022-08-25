using Final.DAL;
using Final.Models;
using Final.ViewModels.Basket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            string cookiebasket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (cookiebasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookiebasket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }
            return View(basketVMs);

        }

        public async Task<IActionResult> GetBasket()
        {
            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }

            return PartialView("_BasketPartial", basketVMs);
        }

        public async Task<IActionResult> GetCard()
        {
            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> Update(int? id, int? count, string color, string size)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                if (!basketVMs.Any(b => b.ProductId == id))
                {
                    return NotFound();
                }

                basketVMs.Find(b => b.ProductId == id).Count = (int)count;
            }
            else
            {
                return BadRequest();
            }

            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookieBasket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> DeleteCard(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id);

                if (basketVM == null)
                {
                    return NotFound();
                }

                basketVMs.Remove(basketVM);
            }
            else
            {
                return BadRequest();
            }

           
            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }
            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            if (User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(cookieBasket))
            {
                AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name.ToUpper() && !u.IsAdmin);
                List<BasketVM> BasketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
                List<Basket> baskets = new List<Basket>();
                List<Basket> existedbaskets = await _context.Baskets.Where(b => b.AppUserId == appUser.Id).ToListAsync();
                for (int i = 0; i < BasketVMs.Count; i++)
                {
                    Basket basket = new Basket
                    {
                        AppUserId = appUser.Id,
                        ProductId = product.Id,
                        Count = basketVMs[i].Count,
                    };
                    baskets.Add(basket);
                }
                _context.RemoveRange(existedbaskets);
                await _context.Baskets.AddRangeAsync(baskets);
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }
        public async Task<IActionResult> DeleteBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id);

                if (basketVM == null)
                {
                    return NotFound();
                }

                basketVMs.Remove(basketVM);
            }
            else
            {
                return BadRequest();
            }

            HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVMs));

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.Image;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

            }


            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            if (User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(cookieBasket))
            {
                AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name.ToUpper() && !u.IsAdmin);
                List<BasketVM> BasketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
                List<Basket> baskets = new List<Basket>();
                List<Basket> existedbaskets = await _context.Baskets.Where(b => b.AppUserId == appUser.Id).ToListAsync();
                for (int i = 0; i < BasketVMs.Count; i++)
                {
                    Basket basket = new Basket
                    {
                        AppUserId = appUser.Id,
                        ProductId = product.Id,
                        Count = basketVMs[i].Count,
                    };
                    baskets.Add(basket);
                }
                _context.RemoveRange(existedbaskets);
                await _context.Baskets.AddRangeAsync(baskets);
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketPartial", basketVMs);
        }
    }
}
