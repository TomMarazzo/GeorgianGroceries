using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Controllers
{
    public class DummiesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "This is a message from the Controller";
            return View("Index");
        }
    }
}
