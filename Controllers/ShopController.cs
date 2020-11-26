using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeorgianGroceries.Models;
using Microsoft.AspNetCore.Http;
using GeorgianGroceries.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

//Stripe payment Ref
using Stripe;
using System.Configuration; // Read the Strip API keys from appssettings.json
using Microsoft.Extensions.Configuration;

namespace GeorgianGroceries.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        // configuration dependency needed to read Stripe Keys from appsettings.json or the secret key store
        private IConfiguration _iconfiguation;

        // this controller uses Depedency Injection - it requires a db connection object when it's created
        public ShopController(ApplicationDbContext context, IConfiguration iconfiguation)
        {
            _context = context;
            _iconfiguation = iconfiguation;
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

        //GET /Shop/Cart
        public IActionResult Cart()
        {
            // fetch current cart for display
            var CustomerId = "";
            // in case user comes to cart page before adding anything, identify them first
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                CustomerId = GetCustomerId();
            }
            else
            {
                CustomerId = HttpContext.Session.GetString("CustomerId");
            }

            // query the db for this customer
            //Add the "Include(c => c.Product)" to have our query include the Parent Products into our cart
            var cartItems = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == CustomerId).ToList();

            // pass the data to the view for display
            return View(cartItems);
        }

        //GET /Shop/RemoveFromCart
        public IActionResult RemoveFromCart(int id)
        {
            // find the item with this PK value
            var cartItem = _context.Carts.Find(id);

            // delete record from Carts table
            if(cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            //redirect to updated Cart
            return RedirectToAction("Cart");
        }

        //Shop/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        //POST: /Shop/Checkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName, LastName, Address, City, Province, PostalCode")] Models.Order order)
        {
            //Populate the 3 automatic Order properties
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;
            //calc order total based on the current cart
            order.Total = (from c in _context.Carts
                           where c.CustomerId == HttpContext.Session.GetString("CustomerId")
                           select c.Quantity * c.Price).Sum();
                            
            //use SessionExtension Obj to store the order Obj in a session variable
            HttpContext.Session.SetObject("Order", order);

            //redirect to Payment Page
            return RedirectToAction("Payment");
        }

        //GET: /Cart/Payment
        [Authorize]
        public IActionResult Payment()
        {
            var order = HttpContext.Session.GetObject<Models.Order>("Order");
            
            ViewBag.Total = order.Total;

            // also use the ViewBag to set the PublishableKey, which we can read from the Configuration
            ViewBag.PublishableKey = _iconfiguation.GetSection("Stripe")["PublishableKey"];

            // load the Payment view
            return View();
        }

        
    }
}
