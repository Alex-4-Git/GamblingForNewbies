using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Written primarily by Vangie.
//Disable by commenting the "customError" tag in web.config (under system.web)
namespace GamblingNewbies.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string param)
        {
            ViewBag.Message = param;
            return View();
        }
    }
}