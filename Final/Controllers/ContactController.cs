using Final.DAL;
using Final.Models;
using Final.ViewModels.Contact;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public ContactController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            Setting setting = _context.Settings.FirstOrDefault();
            Contact contact = _context.Contacts.FirstOrDefault();
            ContactVM contactVM = new ContactVM()
            {
                Setting = setting
            };
            return View(contactVM);
        }

        public async Task<IActionResult> ContactMessage(string Message,string Email, string Name,string Phone)
        {

            //if (!User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("login", "account");
            //}
            if (!User.Identity.IsAuthenticated)
            {
                return Json(0);
            }

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);
            Contact contact = new Contact()
            {
                Email = Email,
                Message = Message,
                Phone = Phone,
                Name = Name
            };

            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "There should be no gaps");
                //return RedirectToAction("index", "contact");

                return PartialView("_ContatctCreatePartial",contact);
            }

            if (string.IsNullOrWhiteSpace(Message))
            {
                ModelState.AddModelError("Message", "There should be no gaps");
                //return RedirectToAction("index", "contact");

                return PartialView("_ContatctCreatePartial",contact);
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "There should be no gaps");
                //return RedirectToAction("index", "contact");

                return PartialView("_ContatctCreatePartial", contact);
            }
            contact.MainEmail = appUser.Email;

            contact.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();

            return PartialView("_ContatctCreatePartial");
        }
    }
}
