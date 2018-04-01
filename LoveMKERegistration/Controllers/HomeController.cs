using LoveMKERegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoveMKERegistration.Controllers
{


    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private bool hasTshirtSignup
        {
            get
            {
                var settings = db.SettingsModels.ToList().First<SettingsModel>();
                return settings.HasTShirts;
            }

            set { }
        }

        public ActionResult Index(string message)
        {
            if(hasTshirtSignup)
            {
                ViewBag.Tshirts = true;
            }
            else
            {
                ViewBag.Tshirts = false;

            }

            ViewBag.Message = message;
        
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}