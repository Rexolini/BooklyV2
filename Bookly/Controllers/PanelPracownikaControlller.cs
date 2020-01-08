using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    public class PanelPracownikaControlller : Controller
    {
        private AppDbContext db = new AppDbContext();
        [Authorize(Roles ="Pracownik")]
        //GET: PanelPracownika
        public ActionResult Index()
        {
            var uzytkownik = db.Users.Where(u => u.EmailConfirmed == false).ToList();
            return View(uzytkownik);
        }
    }
}