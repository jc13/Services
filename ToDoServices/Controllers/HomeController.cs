using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToDoServices.Controllers
{
    public class HomeController : Controller
    {
        //Action Method
        public ActionResult Index()
        {
            ViewBag.Title = "VTS To Do Web API";

            return View();
        }
    }
}
