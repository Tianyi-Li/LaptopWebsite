using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Casestudy.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Casestudy.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(SessionVariables.LoginStatus) == null)
            {
                HttpContext.Session.SetString(SessionVariables.LoginStatus, "Not logged in");
            }
            if (HttpContext.Session.GetString(SessionVariables.LoginStatus) == "Not logged in")
            {
                if (HttpContext.Session.GetString(SessionVariables.Message) == null)
                {
                    HttpContext.Session.SetString(SessionVariables.Message, "Please login!");
                }
            }
            ViewBag.Status = HttpContext.Session.GetString(SessionVariables.LoginStatus);
            ViewBag.Message = HttpContext.Session.GetString(SessionVariables.Message);
            return View();
        }


    }
}
