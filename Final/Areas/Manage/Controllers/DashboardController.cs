using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Areas.Manage.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Manage")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
