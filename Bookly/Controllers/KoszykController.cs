using Bookly.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Bookly.Controllers
{
    [Authorize]
    public class KoszykController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        //GET: Koszyk
        public ActionResult Index()
        {
            Koszyk koszyk;
            if(Session["koszyk"] == null)
            {
                koszyk = new Koszyk();
                Session["koszyk"] = koszyk;
            }
            else
            {
                koszyk = (Koszyk)Session["koszyk"];
            }
            return View(koszyk);
        }

        public ActionResult Wypozycz()
        {
            var koszyk = (Koszyk)Session["koszyk"];
            foreach (var ksiazka in koszyk.Ksiazki)
            {
                var uzytkownikId = ZarzadzanieTozsamoscia.GetUzytkownikById(User.Identity.GetUserId()).Id;

                if(!Wypozyczeniee.MozeZostacWypozyczona(ksiazka.Id_Ksiazka))
                {
                    Session["koszyk"] = null;
                    return RedirectToAction("NieznalezionoKsiazki", "Koszyk", new { ksiazkaId = ksiazka.Id_Ksiazka });
                }
                if(!Wypozyczeniee.JestAktualnieWypozyczona(ksiazka.Id_Ksiazka, uzytkownikId))
                {
                    Session["koszyk"] = null;
                    return RedirectToAction("KsiazkaAktualnieWypozyczona", "Koszyk", new { ksiazkaId = ksiazka.Id_Ksiazka });
                }
                if(!Wypozyczeniee.CzyCzytelnikMozeWypozyczyc(uzytkownikId))
                {
                    Session["koszyk"] = null;
                    return RedirectToAction("ZaDuzoKsiazek", "Koszyk", new { ksiazkaId = ksiazka.Id_Ksiazka });
                }

                var wypozyczanie = new Wypozyczenie()
                {
                    Id_Ksiazka = ksiazka.Id_Ksiazka,
                    Id_Czytelnika = uzytkownikId,
                    WypozyczenieData = DateTime.Today,
                    OstatecznyCzas = DateTime.Today.AddDays(15),
                    ZwrotData = DateTime.Today.AddDays(-1),
                    Status = "Ksiazka w Magazynie"
                };
                db.Wypozyczenia.Add(wypozyczanie);
                db.SaveChanges();
            }

            Session["koszyk"] = null;
            return View();
        }

        public ActionResult NieznalezionoKsiazki(int ksiazkaId)
        {
            var ksiazka = db.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == ksiazkaId);
            return View(ksiazka);
        }

        public ActionResult KsiazkaAktualnieWypozyczona(int ksiazkaId)
        {
            var ksiazka = db.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == ksiazkaId);
            return View(ksiazka);
        }

        public ActionResult ZaDuzoKsiazek(int ksiazkaId)
        {
            var ksiazka = db.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == ksiazkaId);
            return View(ksiazka);
        }

        public ActionResult UsunZKoszyka(string ksiazkaId)
        {
            var id = Int32.Parse(ksiazkaId);
            var koszyk = (Koszyk)Session["koszyk"];
            var ksiazka = koszyk.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == id);
            koszyk.Ksiazki.Remove(ksiazka);
            return RedirectToAction("Index");
        }

        public int IloscKsiazekWKoszyku()
        {
            Koszyk koszyk;
            if(Session ["koszyk"] == null)
            {
                koszyk = new Koszyk();
                Session["koszyk"] = koszyk;
            }
            else
            {
                koszyk = (Koszyk)Session["koszyk"];
            }
            return koszyk.Ksiazki.Count();
        }

    }
}