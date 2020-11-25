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
        IConfiguration _iconfiguation;

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

        //Shop/Cart
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
            var cartCustomer = HttpContext.Session.GetString("CustomerId");
            var cartItems = _context.Carts.Where(c => c.CustomerId == cartCustomer);
            var orderTotal = (from c in cartItems
                              select c.Quantity * c.Price).Sum();
            order.Total = orderTotal;

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
            ViewBag.Total = order.Total * 100; // Stripe charge amount must be in Cents, not Dollars!
            ViewBag.PublishableKey = _iconfiguation["Stripe:PublishableKey"]; //use iconfiguration to read key from appsettings.json
            return View();
        }

        //POST / Cart/Payment
        [Authorize]
        public IActionResult Payment(string stripeToken)
        {
            //retrieve order session
            var order = HttpContext.Session.GetObject<Models.Order>("Order");

            var customerService = new Stripe.CustomerService();
            var charges = new Stripe.ChargeService();

            //1. Create Stripe Customer
            StripeConfiguration.ApiKey = _iconfiguation["Stripe:SecretKey"];
            Stripe.Customer customer = customerService.Create(new Stripe.CustomerCreateOptions
            {
                Source = stripeToken,
                Email = User.Identity.Name
            });
            //2. Create Stripe Charge
            var charge = charges.Create(new Stripe.ChargeCreateOptions
            {
                Amount = Convert.ToInt32(order.Total * 100),
                Description = "Georgian Groceries Purchase",
                Currency = "cad",
                Customer = customer.Id
            }) ;

            //3. Save a new order to our DB
            _context.Orders.Add(order);
            _context.SaveChanges();

            //4. Save the Cart items as new OrderDetails to our DB
            var cartItems = _context.Carts.Where(c => c.CustomerId == HttpContext.Session.GetString("CartUsername"));
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }

            //5. Delete the Cart items from this Order
            foreach (var item in cartItems)
            {
                _context.Carts.Remove(item);
            }

            //6. Load an order confirmation page, without an email!
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });
        }

    }
}
