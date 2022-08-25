using Final.DAL;
using Final.Models;
using Final.ViewModels.Basket;
using Final.ViewModels.Order;
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
    public class OrderController : Controller
    {

        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Create()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null)
            {
                return RedirectToAction("login", "Account");
            }

            double total = 0;
            List<Basket> baskets = await _context.Baskets
                .Include(b => b.Product)
                .Where(b => b.AppUserId == appUser.Id)
                .ToListAsync();

            foreach (Basket item in baskets)
            {
                total = total + (item.Count * (item.Product.Price));
            }

            ViewBag.Total = total;
            ViewBag.Basket = baskets;

            OrderVM orderVM = new OrderVM
            {
                FullName = appUser.FullName,
                Email = appUser.Email,
                Address = appUser.Address,
                City = appUser.City,
                Country = appUser.Country,
                State = appUser.State,
                ZipCode = appUser.ZipCode
            };

            return View(orderVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderVM orderVM)
        {


            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null)
            {
                return RedirectToAction("login", "Account");
            }
            double total = 0;

            List<Basket> baskets = await _context.Baskets
                .Include(b => b.Product)
                .Where(b => b.AppUserId == appUser.Id)
                .ToListAsync();

            foreach (Basket item in baskets)
            {
                total = total + (item.Count * (item.Product.Price));
            }

            ViewBag.Total = total;
            ViewBag.Basket = baskets;

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            List<OrderItem> orderItems = new List<OrderItem>();
           
            foreach (Basket item in baskets)
            {
                total = total + (item.Count * (item.Product.Price));

                OrderItem orderItem = new OrderItem
                {
                    Count = item.Count,
                    Price = (item.Product.Price),
                    ProductId = item.ProductId,
                    TotalPrice = (item.Count * (item.Product.Price)),
                    CreatedAt = DateTime.UtcNow.AddHours(4)
                };
                orderItems.Add(orderItem);
            }

            Order order = new Order
            {
                Address = orderVM.Address,
                AppUserId = appUser.Id,
                City = orderVM.City,
                Country = orderVM.Country,
                State = orderVM.State,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                ZipCode = orderVM.ZipCode,
                OrderItems = orderItems
            };


            _context.Baskets.RemoveRange(baskets);
            HttpContext.Response.Cookies.Append("basket", "");
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", "home");
        }
    }
}
