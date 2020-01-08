using Bookly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bookly.Controllers
{
    [Authorize(Roles = "Pracownik")]
    public class KategorieController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //GET: Kategorie
        public ActionResult Index()
        {
            return View(db.Kategorie.Where(k => k.Id_Kategoria != -1).ToList());
        }

        //GET: Kategoire/Detale/5
        public ActionResult Detale(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategoria kategoria = db.Kategorie.Find(id);
            if(kategoria == null)
            {
                return HttpNotFound();
            }
            return View(kategoria);
        }

        //GET: Kategorie/Utworz
        public ActionResult Utworz()
        {
            ViewBag.PoZaKategoria = new SelectList(db.Kategorie, "KategoriaId", "Nazwa");
            return View();
        }

        //POST: Kategorie/Utworz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Utworz(KategoriaViewModel kategoriaViewModel)
        {
            if(ModelState.IsValid)
            {
                Kategoria kategoria = new Kategoria();
                if (kategoriaViewModel.Id_PoZaKategoria != null)
                {
                    kategoria.Id_PoZaKategoria = kategoriaViewModel.Id_PoZaKategoria.Value;
                    kategoria.PoZaKategoria = db.Kategorie.FirstOrDefault(x => x.Id_Kategoria == kategoria.Id_PoZaKategoria);
                }
                kategoria.Nazwa = kategoriaViewModel.Nazwa;
                db.Kategorie.Add(kategoria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kategoriaViewModel);
        }

        //GET: Kategorie/Edytuj/5
        public ActionResult Edytuj(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategoria kategoria = db.Kategorie.Find(id);
            KategoriaViewModel kategoriaViewModel = new KategoriaViewModel();
            kategoriaViewModel.Id_Kategorii = kategoria.Id_Kategoria;
            kategoriaViewModel.Nazwa = kategoria.Nazwa;
            kategoriaViewModel.Id_PoZaKategoria = kategoria.Id_PoZaKategoria;
            ViewBag.Id_PoZaKategoriami = new SelectList(db.Kategorie.Where(x => x.Id_Kategoria != kategoria.Id_Kategoria), "KategoriaId", "Nazwa");
            if(kategoria == null)
            {
                return HttpNotFound();
            }
            return View(kategoriaViewModel);
        }

        //POST: Kategorie/Edytuj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edytuj(KategoriaViewModel kategoriaViewModel)
        {
            if(ModelState.IsValid)
            {
                var kategoria = db.Kategorie.FirstOrDefault(x => x.Id_Kategoria == kategoriaViewModel.Id_Kategorii);
                if(kategoriaViewModel.Id_PoZaKategoria != null)
                {
                    kategoria.Id_PoZaKategoria = kategoriaViewModel.Id_PoZaKategoria.Value;
                    kategoria.PoZaKategoria = db.Kategorie.FirstOrDefault(x => x.Id_Kategoria == kategoria.Id_PoZaKategoria);
                }
                kategoria.Nazwa = kategoriaViewModel.Nazwa;
                db.Entry(kategoria).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kategoriaViewModel);
        }

        //GET: Kategorie/Usun/5
        public ActionResult Usun(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategoria kategoria = db.Kategorie.Find(id);

            if(kategoria == null)
            {
                return HttpNotFound();
            }
            return View(kategoria);
        }

        //POST: Kategoria/Usun/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PotwierdzenieUsuniecia(int id)
        {
            Kategoria kategoria = db.Kategorie.Find(id);
            int pozakategoria;
            if(kategoria.Id_PoZaKategoria == 0)
            {
                pozakategoria = -1;
            }
            else
            {
                pozakategoria = kategoria.Id_PoZaKategoria;
            }
            kategoria.Id_PoZaKategoria = 0;
            kategoria.PoZaKategoria = null;

            foreach(var ksiazka in db.Ksiazki.ToList())
            {
                if(ksiazka.Id_Kategorii == kategoria.Id_Kategoria)
                {
                    ksiazka.Kategoria = null;
                    ksiazka.Id_Kategorii = pozakategoria;
                    var nowakategoria = db.Kategorie.FirstOrDefault(k => k.Id_Kategoria == pozakategoria);
                    ksiazka.Kategoria = nowakategoria;
                    db.SaveChanges();
                }
            }

            foreach(var dbkategoria in db.Kategorie.ToList())
            {
                if(dbkategoria.Id_PoZaKategoria == kategoria.Id_Kategoria)
                {
                    dbkategoria.PoZaKategoria = null;
                    dbkategoria.Id_PoZaKategoria = 0;
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
            db.Kategorie.Remove(kategoria);
            db.SaveChanges();
            return RedirectToAction("Index");
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