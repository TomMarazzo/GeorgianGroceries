using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GeorgianGroceries.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Browse()
        {
            return View();
        }
    }
}
