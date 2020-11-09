using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeorgianGroceries.Models;
using Microsoft.AspNetCore.Http;
using GeorgianGroceries.Data;
using Microsoft.AspNetCore.Mvc;

namespace GeorgianGroceries.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //Get a list of categories to display to customers on the main shopping page
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(categories);
        }
        //Shop/Browse
        public IActionResult Browse(int id)
        {
            // query Products for the selected Category
            var products = _context.Products.Where(p => p.CategoryId == id).OrderBy(p => p.Name).ToList();

            // get Name of selected Category.  Find() only filters on key fields
            ViewBag.category = _context.Categories.Find(id).Name.ToString();
            return View(products);
        }
    }
}
