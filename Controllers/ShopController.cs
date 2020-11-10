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
        [HttpPost]
        public IActionResult AddToCart(int ProductId, int Quantity)
        {
            // query the db for the product price
            var price = _context.Products.Find(ProductId).Price;

            // get current Date & Time using built in .net function
            var currentDateTime = DateTime.Now;

            // CustomerId variable
            var CustomerId = GetCustomerId();

            // create and save a new Cart object
            var cart = new Cart
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                DateCreated = currentDateTime,
                CustomerId = CustomerId
            };

            _context.Carts.Add(cart);
            _context.SaveChanges();

            // redirect to the Cart view
            return RedirectToAction("Cart");
        }

        private string GetCustomerId()
        {
            // check the session for an existing CustomerId
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                // if we don't already have an existing CustomerId in the session, check if customer is logged in
                var CustomerId = "";

                // if customer is logged in, use their email as the CustomerId
                if (User.Identity.IsAuthenticated)
                {
                    CustomerId = User.Identity.Name; //Name = email address
                }
                // if the customer is anonymous, use Guid to create a new identifier
                else 
                {
                    CustomerId = Guid.NewGuid().ToString();
                }
                // now store the CustomerId in a session variable
                HttpContext.Session.SetString("CustomerId", CustomerId);
            }
            // return the Session variable
            return HttpContext.Session.GetString("CustomerId");
        }

        //Shop/Cart
        public IActionResult Cart()
        {
            return View();
        }
    }
}
