using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeorgianGroceries.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeorgianGroceries.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            //Use fake Category class/model to create and display 10 categories
            //1. Create an Object to hold a list of categories
            var categories = new List<Category>();
            for (var i = 1; i <= 10; i++)
            {
                categories.Add(new Category { CategoryId = i, Name = "Category " + i.ToString() });
            }
            //modify the return View so that it now accepts a list of categories to pass to the view for display
            return View(categories);
        }

        public IActionResult Browse(string category)
        {
            ViewBag.category = category;
            return View();
        }

        // /Store/AddCatyegory
        public IActionResult AddCategory()
        {
            //load a form to capture and object from a user
            return View();
        }
    }
}
