using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    public class MagazynWartosciController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: MagazynWartosci/Edytuj/5
        public ActionResult Edytuj()
        {
            MagazynWartosc magazyn = db.MagazynWartosci.Find(1);
            if(magazyn == null)
            {
                return HttpNotFound();
            }
            return View(magazyn);
        }

        //POST: MagazynWartosci/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edytuj([Bind(Include = "Id, MaksymalneWypozyczenia, MaksymalnyCzasWypozyczenia, CzasNaOdbior, MaksymalnaIloscKsiazekDoOdebrania")]
            MagazynWartosc magazyn)
        {
            if(ModelState.IsValid)
            {
                db.Entry(magazyn).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "PanelPracownika");
            }
            return View(magazyn);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}