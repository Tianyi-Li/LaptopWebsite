using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Casestudy.Utils;
using Casestudy.Models;
using System.Collections.Generic;
using System;
namespace Casestudy.Controllers
{
    public class CartController : Controller
    {
        AppDbContext _db;
        public CartController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult ClearCart() // clear out current tray
        {
            HttpContext.Session.Remove(SessionVariables.Cart);
            HttpContext.Session.SetString(SessionVariables.Message, "Cart Cleared");
            return Redirect("/Home");
        }
        // Add the tray, pass the session variable info to the db
        public ActionResult AddOrder()
        {
            OrderModel model = new OrderModel(_db);
            int retVal = -1;
            string retMessage = "";
            try
            {
                Dictionary<string, object> cartItems = HttpContext.Session.Get<Dictionary<string, object>>(SessionVariables.Cart);
                retVal = model.AddOrder(cartItems, HttpContext.Session.Get<ApplicationUser>(SessionVariables.User));
                // get isBack variable from model class to know if the order is backordered
                bool isBack = model.getIsBack();
                if (retVal > 0) 
                {
                    if(isBack)
                        retMessage = "Order " + retVal + " Created! Some goods were backordered! ";
                    else
                        retMessage = "Order " + retVal + " Created!";
                }
                else // problem
                {
                    retMessage = "Order not added, try again later";
                }
            }
            catch (Exception ex) // big problem
            {
                retMessage = "Order was not created, try again later! - " + ex.Message;
            }
            HttpContext.Session.Remove(SessionVariables.Cart); // clear out current cart once persisted
            HttpContext.Session.SetString(SessionVariables.Message, retMessage);
            return Redirect("/Home");
        }

        [Route("[action]")]
        public IActionResult GetOrders()
        {
            ApplicationUser user = HttpContext.Session.Get<ApplicationUser>(SessionVariables.User);
            OrderModel model = new OrderModel(_db);
            return Ok(model.GetOrdersByUserId(user.Id));
        }

        public IActionResult List()
        {
            // they can't list Orders if they're not logged on
            if (HttpContext.Session.Get<ApplicationUser>(SessionVariables.User) == null)
            {
                return Redirect("/Login");
            }
            return View("List");
        }

        [Route("[action]/{oid:int}")]
        public IActionResult GetOrderDetails(int oid)
        {
            OrderModel model = new OrderModel(_db);
            ApplicationUser user = HttpContext.Session.Get<ApplicationUser>(SessionVariables.User);
            return Ok(model.GetOrderDetails(oid, user.Id));
        }
    }
}
