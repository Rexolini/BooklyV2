using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            var books = db.Ksiazki.ToList().OrderByDescending(b => b.DodajDate).Take(3);
            return View(books);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Yourrrr application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}