using Final.DAL;
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
    public class TableController : Controller
    {
        private readonly AppDbContext _context;
        public TableController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            List<Table> tables = await _context.Tables.ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)tables.Count() / 3);
            return View(tables.Skip((page - 1) * 3).Take(3));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Table table = await _context.Tables
                .FirstOrDefaultAsync(r => r.Id == id);
            if (table == null) return NotFound();

            return View(table);
        }
    }
}
