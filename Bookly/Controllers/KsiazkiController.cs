using Bookly.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Plik = Bookly.Models.Plik;

namespace Bookly.Controllers
{
    public class KsiazkiController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        private void SetViewBag()
        {
            var listopcje = new List<string>() { "or", "and", "not" };

            ViewBag.Opcje = new SelectList(listopcje);
            ViewBag.Etykiety = new SelectList(db.Etykiety, "EtykietyId", "Nazwa");
            ViewBag.Kategories = new SelectList(db.Kategorie, "KategorieId", "Nazwa");
            IEnumerable<SelectListItem> pisarze = from p in db.Pisarze
                                                  select new SelectListItem
                                                  {
                                                      Value = p.Id_Pisarza.ToString(),
                                                      Text = p.Imie + " " + p.Nazwisko
                                                  };
            ViewBag.Pisarze = new SelectList(pisarze, "Value", "Text");

        }

        //GET: Ksiazki

        public ActionResult Index()
        {
            var savm = new SzukajArchiwumViewModel();
            var ksiazki = db.Ksiazki.ToList();
            savm.Ksiazki = ksiazki;
            SetViewBag();
            return View(savm);
        }

        public IEnumerable<Models.Ksiazka> SzukajTytul(string tytul, bool nie)
        {
            IEnumerable<Models.Ksiazka> ksiazki = db.Ksiazki.Where(k => k.Tytul.Contains(tytul)).ToList();
            if(nie)
            {
                ksiazki = db.Ksiazki.ToList().Except(ksiazki).ToList();
            }
            return ksiazki;
        }

        public IEnumerable<Models.Ksiazka> SzukajKategori(int kategoria, bool nie)
        {
            IEnumerable<Models.Ksiazka> ksiazki = db.Ksiazki.Where(k => k.Id_Kategorii == kategoria).ToList();
            if(nie)
            {
                ksiazki = db.Ksiazki.ToList().Except(ksiazki).ToList();
            }
            return ksiazki;
        }

        public IEnumerable<Models.Ksiazka> SzukajPisarza(int[] pisarze, string opcje)
        {
            IEnumerable<Models.Ksiazka> ksiazki = new List<Models.Ksiazka>();
            List<List<Models.Ksiazka>> listaksiazek = new List<List<Models.Ksiazka>>();
            if(opcje == "or")
            {
                ICollection<Models.Ksiazka> lubKsiazki = new List<Models.Ksiazka>();
                foreach(var szukajpisarza in pisarze)
                {
                    var pisarz = db.Pisarze.FirstOrDefault(p => p.Id_Pisarza == szukajpisarza);
                    if(pisarz != null)
                    {
                        int pisarzitem = pisarz.Id_Pisarza;
                        var odpowiednieksiazki = db.Ksiazki.Where(ok => ok.Id_Ksiazka == pisarzitem);
                        foreach(var odpowiedniaksiazka in odpowiednieksiazki)
                        {
                            if(!ksiazki.Contains(odpowiedniaksiazka))
                            {
                                lubKsiazki.Add(odpowiedniaksiazka);
                            }
                        }
                    }
                }
                ksiazki = lubKsiazki;

            }
            else if(opcje == "and")
            {
                foreach(var szukajpisarza in pisarze)
                {
                    List<Models.Ksiazka> lokalneksiazki = new List<Models.Ksiazka>();
                    var pisarz = db.Pisarze.FirstOrDefault(p => p.Id_Pisarza == szukajpisarza);
                    if(pisarz != null)
                    {
                        int pisarzitem = pisarz.Id_Pisarza;
                        var odpowiednieksiazki = db.Ksiazki.Where(k => k.Pisarze.Any(ok => ok.Id_Pisarz == pisarzitem));
                        foreach(var odpowiedniaksiazka in odpowiednieksiazki)
                        {
                            if(!ksiazki.Contains(odpowiedniaksiazka))
                            {
                                lokalneksiazki.Add(odpowiedniaksiazka);
                            }
                        }
                    }
                    listaksiazek.Add(lokalneksiazki);
                }
                ksiazki = listaksiazek[0];
                for(int i = 1; i < listaksiazek.Count; i++)
                {
                    ksiazki = ksiazki.Intersect(listaksiazek[i]).ToList();
                }
            }
            else
            {
                ICollection<Models.Ksiazka> nieKsiazki = new List<Models.Ksiazka>();
                foreach(var szukajpisarza in pisarze)
                {
                    var pisarz = db.Pisarze.FirstOrDefault(p => p.Id_Pisarza == szukajpisarza);
                    if(pisarz != null)
                    {
                        int pisarzitem = pisarz.Id_Pisarza;
                        var odopowiednieksiazki = db.Ksiazki.Where(k => k.Pisarze.Any(ok => ok.Id_Pisarz == pisarzitem));
                        foreach(var odpowiedniaksiazka in odopowiednieksiazki)
                        {
                            if(!ksiazki.Contains(odpowiedniaksiazka))
                            {
                                nieKsiazki.Add(odpowiedniaksiazka);
                            }
                        }
                    }
                }
                ksiazki = db.Ksiazki.ToList().Except(nieKsiazki).ToList();
            }
            return ksiazki;
        }

        public IEnumerable<Models.Ksiazka> SzukajISBN(string isbn, bool nie)
        {
            IEnumerable<Ksiazka> ksiazki = db.Ksiazki.Where(k => k.ISBN == isbn).ToList();
            if (nie)
            {
                ksiazki = db.Ksiazki.ToList().Except(ksiazki).ToList();
            }
            return ksiazki;
        }

        public IEnumerable<Ksiazka> SzukajRok(int rok, bool nie)
        {
            IEnumerable<Ksiazka> ksiazki = db.Ksiazki.Where(k => k.Rok == rok).ToList();
            if (nie)
            {
                ksiazki = db.Ksiazki.ToList().Except(ksiazki).ToList();
            }
            var svm = new SzukajArchiwumViewModel();
            return ksiazki;
        }

        public IEnumerable<Models.Ksiazka> SzukajEtykiet(int[] etykiety, string opcje)
        {
            IEnumerable<Models.Ksiazka> ksiazki = new List<Models.Ksiazka>();
            List<List<Models.Ksiazka>> listaksiazek = new List<List<Models.Ksiazka>>();
            if(opcje == "or")
            {
                ICollection<Models.Ksiazka> lubKsiazki = new List<Models.Ksiazka>();
                foreach(var szukajetykieta in etykiety)
                {
                    var etykieta = db.Etykiety.FirstOrDefault(e => e.Id_Etykieta == szukajetykieta);
                    if(etykieta != null)
                    {
                        int etykietaitem = etykieta.Id_Etykieta;
                        var odpowiednieksiazki = db.Ksiazki.Where(k => k.Etykiety.Any(ok => ok.Etykieta.Id_Etykieta == etykietaitem));
                        foreach(var odpowiedniaksiazka in odpowiednieksiazki)
                        {
                            if(!ksiazki.Contains(odpowiedniaksiazka))
                            {
                                lubKsiazki.Add(odpowiedniaksiazka);
                            }
                        }

                    }
                }
                ksiazki = lubKsiazki;
            }
            else if(opcje == "and")
            {
                foreach(var szukajetykieta in etykiety)
                {
                    List<Models.Ksiazka> lokalneksiazki = new List<Models.Ksiazka>();
                    var etykieta = db.Etykiety.FirstOrDefault(e => e.Id_Etykieta == szukajetykieta);
                    if(etykieta != null)
                    {
                        int etykietaitem = etykieta.Id_Etykieta;
                        var odpowiednieksiazki = db.Ksiazki.Where(k => k.Etykiety.Any(ok => ok.Etykieta.Id_Etykieta == etykietaitem));
                        foreach(var odpowiedniaksiazka in odpowiednieksiazki)
                        {
                            if(!lokalneksiazki.Contains(odpowiedniaksiazka))
                            {
                                lokalneksiazki.Add(odpowiedniaksiazka);
                            }
                        }
                    }
                    listaksiazek.Add(lokalneksiazki);
                        
                }
                ksiazki = listaksiazek[0];
                for(int i = 1; i < listaksiazek.Count; i++)
                {
                    ksiazki = ksiazki.Intersect(listaksiazek[i]).ToList();
                }
            }
            else
            {
                ICollection<Models.Ksiazka> nieKsiazki = new List<Models.Ksiazka>();
                foreach (var szukajetykieta in etykiety)
                {
                    var etykieta = db.Etykiety.FirstOrDefault(e => e.Id_Etykieta == szukajetykieta);
                    if (etykieta != null)
                    {
                        int etykietaitem = etykieta.Id_Etykieta;
                        var odpowiednieksiazki = db.Ksiazki.Where(k => k.Etykiety.Any(ok => ok.Etykieta.Id_Etykieta == etykietaitem));
                        foreach (var odpowiedniaksiazka in odpowiednieksiazki)
                        {
                            if (!ksiazki.Contains(odpowiedniaksiazka))
                            {
                                nieKsiazki.Add(odpowiedniaksiazka);
                            }
                        }
                    }
                }
                ksiazki = db.Ksiazki.ToList().Except(nieKsiazki).ToList();
            }
            return ksiazki;
        }
        
        

        public List<IEnumerable<Ksiazka>> SzukajWszystkie(SzukajArchiwumViewModel savm)
        {
            IEnumerable<Ksiazka> tytulyKsiazek = null;
            IEnumerable<Ksiazka> kategorieKsiazek = null;
            IEnumerable<Ksiazka> pisarzeKsiazek = null;
            IEnumerable<Ksiazka> etykietyKsiazek = null;
            IEnumerable<Ksiazka> rokKsiazek = null;
            IEnumerable<Ksiazka> ISBNksiazek = null;
            if (!string.IsNullOrEmpty(savm.WybranyTytul))
            {
                tytulyKsiazek = SzukajTytul(savm.WybranyTytul, savm.TytulOpcja);
            }
            if (savm.WybranaKategoria != 0)
            {
                kategorieKsiazek = SzukajKategori(savm.WybranaKategoria, savm.KategoriaOpcja);
            }
            if (savm.WybranyRok != 0)
            {
                rokKsiazek = SzukajRok(savm.WybranyRok, savm.RokOpcja);
            }
            if (!string.IsNullOrEmpty(savm.WybranyISBN))
            {
                ISBNksiazek = SzukajISBN(savm.WybranyISBN, savm.ISBNOpcja);
            }
            if (savm.WybraneEtykiety != null)
            {
                etykietyKsiazek = SzukajEtykiet(savm.WybraneEtykiety, savm.EtykietyOpcja);
            }
            if (savm.WybranyPisarz != null)
            {
                pisarzeKsiazek = SzukajPisarza(savm.WybranyPisarz, savm.PisarzeOpcja);
            }
            
            return new List<IEnumerable<Ksiazka>>() { tytulyKsiazek, pisarzeKsiazek, kategorieKsiazek, ISBNksiazek, rokKsiazek, etykietyKsiazek };

        }

        private IEnumerable<Ksiazka> WszystkieI(List<IEnumerable<Ksiazka>> listaksiazek)
        {
            IEnumerable<Ksiazka> ksiazki = null;
            foreach (var listaksiazki in listaksiazek)
            {
                if (listaksiazki != null)
                {
                    ksiazki = listaksiazki;
                    break;
                }
            }
            if (ksiazki != null)
            {
                foreach (IEnumerable<Ksiazka> listaksiazki in listaksiazek)
                {
                    if (listaksiazki != null)
                    {
                        ksiazki = ksiazki.Intersect(listaksiazki).ToList();
                    }
                }
            }
            return ksiazki;
        }
        private IEnumerable<Ksiazka> WszystkieNie(List<IEnumerable<Ksiazka>> listaksiazek)
        {
            IEnumerable<Ksiazka> ksiazki = db.Ksiazki.ToList();
            for (int i = 0; i < listaksiazek.Count; i++)
            {
                if (listaksiazek[i] != null)
                {
                    ksiazki = ksiazki.Except(listaksiazek[i]).ToList();
                }
            }
            return ksiazki;
        }

        private IEnumerable<Ksiazka> WszystkieLub(List<IEnumerable<Ksiazka>> listaksiazek)
        {
            IEnumerable<Ksiazka> ksiazki = null;
            for (int i = 0; i < listaksiazek.Count; i++)
            {
                if (listaksiazek[i] != null)
                {
                    ksiazki = listaksiazek[i];
                    break;
                }
            }
            if (ksiazki != null)
            {
                for (int i = 1; i < listaksiazek.Count; i++)
                {
                    if (listaksiazek[i] != null)
                    {
                        ksiazki = ksiazki.Union(listaksiazek[i]).ToList();
                    }
                }
            }

            return ksiazki;
        }
        


        //Post: Ksiazki
        [HttpPost]
        public ActionResult Index(SzukajArchiwumViewModel savm)
        {
            SetViewBag();
            if (Request.Form["Tytul"] != null)
            {
                savm.Ksiazki = SzukajTytul(savm.WybranyTytul, savm.TytulOpcja);
                return View("Index", savm);
            }
            if (Request.Form["Kategorie"] != null)
            {
                savm.Ksiazki = SzukajKategori(savm.WybranaKategoria, savm.KategoriaOpcja);
                return View("Index", savm);
            }
            if (Request.Form["Pisarze"] != null)
            {
                savm.Ksiazki = SzukajPisarza(savm.WybranyPisarz, savm.PisarzeOpcja);
                return View("Index", savm);
            }
            if (Request.Form["Etykiety"] != null)
            {
                savm.Ksiazki = SzukajEtykiet(savm.WybraneEtykiety, savm.EtykietyOpcja);
                return View("Index", savm);
            }
            if (Request.Form["Rok"] != null)
            {
                savm.Ksiazki = SzukajRok(savm.WybranyRok, savm.RokOpcja);
                return View("Index", savm);
            }
            if (Request.Form["ISBN"] != null)
            {
                savm.Ksiazki = SzukajISBN(savm.WybranyISBN, savm.ISBNOpcja);
                return View("Index", savm);
            }
            if (Request.Form["Wszystkie"] != null)
            {
                var listaksiazek = SzukajWszystkie(savm);
                switch (savm.WszystkieOpcje)
                {
                    case "or":
                        savm.Ksiazki = WszystkieLub(listaksiazek);
                        break;
                    case "and":
                        savm.Ksiazki = WszystkieI(listaksiazek);
                        break;
                    default:
                    case "not":
                        savm.Ksiazki = WszystkieNie(listaksiazek);
                        break;
                }
                return View("Index", savm);
            }
            if (Request.Form["Zapisz"] != null)
            {
                Save(savm);
                savm.Ksiazki = db.Ksiazki.ToList();
            }
            return View(savm);
        }
        [Authorize]
        public void Save(SzukajArchiwumViewModel savm)
        {
            //json
            var serializer = new JavaScriptSerializer();
            var serializerobject = serializer.Serialize(savm);
            var sa = new SzukajArchiwum();
            sa.Nazwa = savm.ZapiszNazwe;
            sa.Id_Czytelnika = User.Identity.GetUserId();
            sa.URL = serializerobject;
            db.Archiwa.Add(sa);
            db.SaveChanges();
        }
        [Authorize]
        public ActionResult Wczytaj(int id)
        {
            var przeszukajHistorie = db.Archiwa.FirstOrDefault(a => a.Id_Archiwum == id);
            var serializer = new JavaScriptSerializer();
            var savm = serializer.Deserialize<SzukajArchiwumViewModel>(przeszukajHistorie.URL);
            savm.Ksiazki = db.Ksiazki.ToList();

            var listaksiazek = SzukajWszystkie(savm);
            switch (savm.WszystkieOpcje)
            {
                case "or":
                    savm.Ksiazki = WszystkieLub(listaksiazek);
                    break;
                case "and":
                    savm.Ksiazki = WszystkieI(listaksiazek);
                    break;
                default:
                    savm.Ksiazki = WszystkieNie(listaksiazek);
                    break;
            }
            SetViewBag();
            return View("Index", savm);
        }
        // GET: Ksiazki/Detale/5
        public ActionResult Detale(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ksiazka = db.Ksiazki.Find(id);
            var directory = Path.Combine(Server.MapPath("~/App_Data/uploads"), ksiazka.Id_Ksiazka.ToString());
            ViewBag.Content = System.IO.File.ReadAllText(Path.Combine(directory, ksiazka.Zawartosc), Encoding.Default);
            if (ksiazka == null)
                return HttpNotFound();
            return View(ksiazka);
        }
        [Authorize(Roles = "Pracownik")]
        // GET: Ksiazki/Stworz
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> piszarze = from p in db.Pisarze
                                                  select new SelectListItem
                                                  {
                                                      Value = p.Id_Pisarza.ToString(),
                                                      Text = p.Imie + " " + p.Nazwisko
                                                  };
            ViewBag.Id_Kategorii = new SelectList(db.Kategorie, "KategoriaId", "Nazwa");
            ViewBag.Pisarze = new SelectList(piszarze, "Value", "Text");
            ViewBag.Etykiety = new SelectList(db.Etykiety, "EtykietyId", "Nazwa");
            return View();
        }

        // POST: Ksiazki/Stowrz
        
        [Authorize(Roles = "Pracownik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KsiazkaViewModel kvm)
        {

            if (ModelState.IsValid)
            {
                var rozbudowa = Path.GetExtension(kvm.Zawartosc.FileName);

                Ksiazka ksiazka = new Ksiazka()
                {
                    DodajDate = DateTime.Now,
                    Naklad = kvm.Naklad,
                    Id_Kategorii = kvm.Id_Kategorii,
                    Rok = kvm.Rok,
                    Tytul = kvm.Tytul,
                    ISBN = kvm.ISBN,
                    Opis = kvm.Opis,
                    Pisarze = new List<KsiazkaPisarz>(),
                    Etykiety = new List<KsiazkaEtykieta>()

                };
                db.Ksiazki.Add(ksiazka);
                db.SaveChanges();
                var nazwapliku = ksiazka.Id_Ksiazka + "zawartosc" + rozbudowa;
                ksiazka.Zawartosc = nazwapliku;
                var directory = Path.Combine(Server.MapPath("~/App_Data/uploads"), ksiazka.Id_Ksiazka.ToString());
                Directory.CreateDirectory(directory);
                var sciezka = Path.Combine(directory, nazwapliku);
                kvm.Zawartosc.SaveAs(sciezka);
                db.SaveChanges();
                for (int i = 0; i < kvm.WybraniPisarze.Length; i++)
                {
                    var id = kvm.WybraniPisarze[i];
                    var pisarz = db.Pisarze.FirstOrDefault(x => x.Id_Pisarza == id);
                    if (pisarz != null)
                    {
                        var y = new KsiazkaPisarz()
                        {
                            Id_Ksiazka = ksiazka.Id_Ksiazka,
                            Id_KsiazkaPisarz = pisarz.Id_Pisarza
                        };
                        db.KsiakaPisarze.Add(y);
                        db.SaveChanges();
                    }
                }
                for (int i = 0; i < kvm.WybraneEtykiety.Length; i++)
                {
                    var id = kvm.WybraneEtykiety[i];
                    var etykiety = db.Etykiety.FirstOrDefault(x => x.Id_Etykieta == id);
                    if (etykiety != null)
                    {
                        var ke = new KsiazkaEtykieta()
                        {
                            Id_Ksiazki = ksiazka.Id_Ksiazka,
                            Id_Etykiety = etykiety.Id_Etykieta
                        };
                        db.KsiazkaEtykiety.Add(ke);
                        db.SaveChanges();
                    }
                }
                db.Entry(ksiazka).State = EntityState.Modified;
                db.SaveChanges();

                for (var i = 0; i < kvm.Pliki.Count; i++)
                {
                    if (kvm.Pliki[i] != null)
                    {
                        Plik plik = new Plik();
                        plik.Id_Ksiazka = ksiazka.Id_Ksiazka;
                        plik.Ksiazka = ksiazka;
                        plik.Nazwa = kvm.PlikiNazwy[i];
                        plik.Zrodlo = kvm.Pliki[i].FileName;
                        kvm.Pliki[i].SaveAs(Path.Combine(directory, kvm.Pliki[i].FileName));
                        db.Pliki.Add(plik);
                        db.SaveChanges();
                    }
                }


                return RedirectToAction("Index");
            }


            IEnumerable<SelectListItem> pisarze = from p in db.Pisarze
                                                  select new SelectListItem
                                                  {
                                                      Value = p.Id_Pisarza.ToString(),
                                                      Text = p.Imie + " " + p.Nazwisko
                                                  };
            ViewBag.Id_Kategorii = new SelectList(db.Kategorie, "KategoriaId", "Nazwa");
            ViewBag.Pisarze = new SelectList(pisarze, "Value", "Text");
            ViewBag.Etykiety = new SelectList(db.Etykiety, "EtykietyId", "Nazwa");
            return View(kvm);
        }

        // GET: Ksiazki/Edytuj/5
        [Authorize(Roles = "Pracownik")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ksiazka = db.Ksiazki
                    .Include(b => b.Pisarze)
                    .Include(b => b.Etykiety)
                    .Include(b => b.Kategoria)
                    .Single(b => b.Id_Ksiazka == id);

            var plikilista = db.Pliki.Where(f => f.Id_Ksiazka == ksiazka.Id_Ksiazka);
            var plikilistazrodla = plikilista.Select(f => f.Zrodlo).ToList();
            var plikilistatekstu = plikilista.Select(f => f.Nazwa).ToList();

            var kevm = new KsiazkaEdytujViewModel
            {
                KsiazkaViewModel = new KsiazkaViewModel()
                {
                    Naklad = ksiazka.Naklad,
                    Id_Ksiazka = ksiazka.Id_Ksiazka,
                    Id_Kategorii = ksiazka.Id_Kategorii,
                    Opis = ksiazka.Opis,
                    ISBN = ksiazka.ISBN,
                    Tytul = ksiazka.Tytul,
                    Rok = ksiazka.Rok,
                    Pliki = new List<HttpPostedFileBase>(),

                },
                StaraZawartosc = ksiazka.Zawartosc,
                StarePliki = plikilistazrodla,
                StarePlikiTekst = plikilistatekstu
            };
            kevm.KsiazkaViewModel.WybraneEtykiety = new int[db.Etykiety.Count()];
            kevm.KsiazkaViewModel.WybraniPisarze = new int[db.Pisarze.Count()];
            for (int i = 0; i < ksiazka.Pisarze.Count; i++)
            {
                kevm.KsiazkaViewModel.WybraniPisarze[i] = ksiazka.Pisarze.ElementAt(i).Id_Pisarz;
            }
            for (int i = 0; i < ksiazka.Etykiety.Count; i++)
            {
                kevm.KsiazkaViewModel.WybraneEtykiety[i] = ksiazka.Etykiety.ElementAt(i).Id_Etykiety;
            }

            if (ksiazka == null)
                return HttpNotFound();
            IEnumerable<SelectListItem> pisarze = from p in db.Pisarze
                                                  select new SelectListItem
                                                  {
                                                      Value = p.Id_Pisarza.ToString(),
                                                      Text = p.Imie + " " + p.Nazwisko
                                                  };
            ViewBag.Id_Kategorii = new SelectList(db.Kategorie, "KategoriaId", "Nazwa");
            ViewBag.Pisarze = new SelectList(pisarze, "Value", "Text");
            ViewBag.Etykiety = new SelectList(db.Etykiety, "EtykietyId", "Nazwa");
            return View(kevm);
        }

        // POST: Ksiazki/Edytuj/5
        
        [Authorize(Roles = "Pracownik")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edytuj(KsiazkaEdytujViewModel kevm)
        {
            if (kevm.KsiazkaViewModel.Zawartosc == null && !string.IsNullOrEmpty(kevm.StaraZawartosc))
            {
                ModelState["KsiazkaViewModel.Zawartosc"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {

                var ksiazka = db.Ksiazki
                    .Include(k => k.Pisarze)
                    .Include(k => k.Etykiety)
                    .Include(k => k.Kategoria)
                    .Single(k => k.Id_Ksiazka == kevm.KsiazkaViewModel.Id_Ksiazka);

                var directory = Path.Combine(Server.MapPath("~/App_Data/uploads"), ksiazka.Id_Ksiazka.ToString());
                string pliknazwa;
                if (kevm.KsiazkaViewModel.Zawartosc != null)
                {
                    var starasciezkapliku = Path.Combine(directory, ksiazka.Zawartosc);
                    if (System.IO.File.Exists(starasciezkapliku))
                    {
                        System.IO.File.Delete(starasciezkapliku);
                    }
                    var rozbudowanie = Path.GetExtension(kevm.KsiazkaViewModel.Zawartosc.FileName);
                    pliknazwa = kevm.KsiazkaViewModel.Id_Ksiazka + "zawartosc" + rozbudowanie;
                    kevm.KsiazkaViewModel.Zawartosc.SaveAs(Path.Combine(directory, pliknazwa));
                }
                else
                {
                    pliknazwa = kevm.StaraZawartosc;
                }

                ksiazka.Naklad = kevm.KsiazkaViewModel.Naklad;
                ksiazka.Zawartosc = pliknazwa;
                ksiazka.Tytul = kevm.KsiazkaViewModel.Tytul;
                ksiazka.ISBN = kevm.KsiazkaViewModel.ISBN;
                ksiazka.Rok = kevm.KsiazkaViewModel.Rok;
                ksiazka.Opis = kevm.KsiazkaViewModel.Opis;
                ksiazka.Id_Kategorii = kevm.KsiazkaViewModel.Id_Kategorii;
                ksiazka.Kategoria = db.Kategorie.FirstOrDefault(x => x.Id_Kategoria == kevm.KsiazkaViewModel.Id_Kategorii);
                var wybranipisarzelista = new List<Pisarz>();
                var wybranieetykietylista = new List<Etykieta>();

                for (int i = 0; i < kevm.KsiazkaViewModel.WybraniPisarze.Length; i++)
                {
                    var id = kevm.KsiazkaViewModel.WybraniPisarze[i];
                    var pisarz = db.Pisarze.FirstOrDefault(x => x.Id_Pisarza == id);
                    if (pisarz != null)
                    {
                        wybranipisarzelista.Add(pisarz);
                    }
                }
                db.SaveChanges();
                for (int i = 0; i < kevm.KsiazkaViewModel.WybraneEtykiety.Length; i++)
                {
                    var id = kevm.KsiazkaViewModel.WybraneEtykiety[i];
                    var etykieta = db.Etykiety.FirstOrDefault(x => x.Id_Etykieta == id);
                    if (etykieta != null)
                    {
                        wybranieetykietylista.Add(etykieta);
                    }
                }

                //Sprawdzenie Pisarzy
                var aktualnypisarz = db.Pisarze.Where(w => w.KsiazkaPisarze.Any(b => b.Id_Ksiazka == kevm.KsiazkaViewModel.Id_Ksiazka)).ToList();
                db.SaveChanges();
                foreach (var dbpisarz in db.Pisarze.ToList())
                {
                    if (wybranipisarzelista.Contains(dbpisarz))
                    {
                        if (!aktualnypisarz.Contains(dbpisarz))
                        {
                            var kp = new KsiazkaPisarz()
                            {
                                Id_Pisarz = dbpisarz.Id_Pisarza,
                                Id_Ksiazka = kevm.KsiazkaViewModel.Id_Ksiazka
                            };
                            db.KsiakaPisarze.Add(kp);
                        }
                    }
                    else
                    {
                        if (aktualnypisarz.Contains(dbpisarz))
                        {
                            var kp =
                                db.KsiakaPisarze.FirstOrDefault(
                                    x => x.Id_Pisarz == dbpisarz.Id_Pisarza && x.Id_Ksiazka == kevm.KsiazkaViewModel.Id_Ksiazka);
                            db.KsiakaPisarze.Remove(kp);
                        }
                    }
                    db.SaveChanges();
                }

                //Sprawdzenie etykiet
                var aktualnaetykieta = db.Etykiety.Where(w => w.KsiazkaEtykiety.Any(b => b.Id_Ksiazki == kevm.KsiazkaViewModel.Id_Ksiazka)).ToList();
                db.SaveChanges();
                foreach (var dbEtykiety in db.Etykiety.ToList())
                {
                    if (wybranieetykietylista.Contains(dbEtykiety))
                    {
                        if (!aktualnaetykieta.Contains(dbEtykiety))
                        {
                            var ke = new KsiazkaEtykieta()
                            {
                                Id_Ksiazki = kevm.KsiazkaViewModel.Id_Ksiazka,
                                Id_Etykiety = dbEtykiety.Id_Etykieta
                            };
                            db.KsiazkaEtykiety.Add(ke);
                        }

                    }
                    else
                    {
                        if (aktualnaetykieta.Contains(dbEtykiety))
                        {
                            var ke =
                                db.KsiazkaEtykiety.FirstOrDefault
                                (x => x.Id_Etykiety == dbEtykiety.Id_Etykieta && x.Id_Ksiazki == kevm.KsiazkaViewModel.Id_Ksiazka);
                            db.KsiazkaEtykiety.Remove(ke);
                        }
                    }
                    db.SaveChanges();
                }

                //Sprawdzanie Plikow
                List<string> edytujzrodlopliku;
                List<string> edytujtesktpliku;
                if (kevm.StarePliki != null)
                {
                    edytujzrodlopliku = kevm.StarePliki;
                }
                else
                {
                    edytujzrodlopliku = new List<string>();
                }
                if (kevm.StarePlikiTekst != null)
                {
                    edytujtesktpliku = kevm.StarePlikiTekst;
                }
                else
                {
                    edytujtesktpliku = new List<string>();
                }
                var wszystkiepliki = db.Pliki.Where(f => f.Id_Ksiazka == kevm.KsiazkaViewModel.Id_Ksiazka).ToList();
                var wszystkieplikizrodlo = wszystkiepliki.Select(f => f.Zrodlo).ToList();

               
                if (kevm.KsiazkaViewModel.Pliki != null)
                {
                    for (var i = 0; i < kevm.KsiazkaViewModel.Pliki.Count; i++)
                    {
                        if (kevm.KsiazkaViewModel.Pliki[i] != null)
                        {
                            var zrodlo = kevm.KsiazkaViewModel.Pliki[i].FileName;
                            var sciezka = Path.Combine(directory, zrodlo);
                            kevm.KsiazkaViewModel.Pliki[i].SaveAs(sciezka);
                            edytujzrodlopliku.Add(zrodlo);
                            edytujtesktpliku.Add(kevm.KsiazkaViewModel.PlikiNazwy[i]);
                        }
                    }
                }

                foreach (var dbplik in wszystkiepliki)
                {
                   
                    if (!edytujzrodlopliku.Contains(dbplik.Zrodlo))
                    {
                        if (System.IO.File.Exists(Path.Combine(directory, dbplik.Zrodlo)))
                        {
                            System.IO.File.Delete(Path.Combine(directory, dbplik.Zrodlo));
                        }

                        db.Pliki.Remove(dbplik);
                    }
                }

                if (edytujzrodlopliku.Count > 0)
                {
                    for (var i = 0; i < edytujzrodlopliku.Count; i++)
                    {
                        
                        if (!wszystkieplikizrodlo.Contains(edytujzrodlopliku[i]))
                        {
                            Plik plik = new Plik()
                            {
                                Id_Ksiazka = kevm.KsiazkaViewModel.Id_Ksiazka,
                                Nazwa = edytujtesktpliku[i],
                                Zrodlo = edytujzrodlopliku[i]
                            };
                            db.Pliki.Add(plik);
                            db.SaveChanges();
                        }
                        else
                        {
                            
                            var zrodlo = edytujzrodlopliku[i];
                            var pliktekst =
                            db.Pliki.FirstOrDefault(
                                f => f.Id_Ksiazka == kevm.KsiazkaViewModel.Id_Ksiazka && f.Zrodlo == zrodlo);

                            
                            if (edytujtesktpliku != null)
                            {
                                if (edytujtesktpliku[i] != pliktekst.Nazwa)
                                {
                                    pliktekst.Nazwa = edytujtesktpliku[i];
                                    db.SaveChanges();
                                }
                            }
                        }
                       
                        db.SaveChanges();
                    }
                }


                db.Entry(ksiazka).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            SetViewBag();
            return View(kevm);
        }

        [HttpGet]
        public ActionResult KsiazkaZbiory()
        {
            var listazbiorow = new List<KsiazkaZbioryViewModel>();
            var ksiazki = db.Ksiazki.ToList();
            foreach (var ksiazka in ksiazki)
            {
                var wypozyczona = db.Wypozyczenia.Where(w => w.Id_Ksiazka == ksiazka.Id_Ksiazka && w.ZwrotData < w.WypozyczenieData).ToList().Count;
                var zbiorksiazek = new KsiazkaZbioryViewModel()
                {
                    Naklad = ksiazka.Naklad,
                    Id_KsiazkaZbiory = ksiazka.Id_Ksiazka,
                    ISBN = ksiazka.ISBN,
                    Tytul = ksiazka.Tytul,
                    Wypozyczone = wypozyczona
                };
                listazbiorow.Add(zbiorksiazek);
            }
            return View(listazbiorow);
        }
        // GET: Ksiazki/Usun/5

        [Authorize(Roles = "Pracownik")]
        public ActionResult Usun(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ksiazka = db.Ksiazki.Find(id);
            if (ksiazka == null)
                return HttpNotFound();
            return View(ksiazka);
        }

        // POST: Ksiazki/Usun/5
        [Authorize(Roles = "Pracownik")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult PotwierdzenieUsuniecia(int id)
        {
            var ksiazka = db.Ksiazki.Find(id);
            db.Ksiazki.Remove(ksiazka);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
        [Authorize]
        [ActionName("Pobierz")]
        public void Pobierz(string pliknazwa, string ksiazkaid)
        {
            //na podstawie strony microsoftu
            var directory = Path.Combine(Path.Combine("/App_Data/uploads", ksiazkaid));
            var sciezkapliku = Path.Combine(directory, pliknazwa);
            System.Web.HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
            var header = "Powiazanie; NazwaPliku=" + pliknazwa;
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", header);
            var dfile = new FileInfo(System.Web.HttpContext.Current.Server.MapPath(sciezkapliku));
            System.Web.HttpContext.Current.Response.WriteFile(dfile.FullName);
            System.Web.HttpContext.Current.Response.End();
        }
        [Authorize]
        [ActionName("DodajDokoszyka")]
        public ActionResult DodajDokoszyka(string ksiazkaid)
        {
            int id = Int32.Parse(ksiazkaid);
            var ksiazka = db.Ksiazki.FirstOrDefault(k => k.Id_Ksiazka == id);
            Koszyk koszyk;
            if (Session["basket"] == null)
            {
                koszyk = new Koszyk();
                Session["koszyk"] = koszyk;
            }
            else
            {
                koszyk = (Koszyk)Session["koszyk"];
            }
            bool contains = koszyk.Ksiazki.Any(k => k.Id_Ksiazka == ksiazka.Id_Ksiazka);
            if (!contains)
            {
                koszyk.Ksiazki.Add(ksiazka);
            }
            return RedirectToAction("Index", "Basket");
        }
        [Authorize(Roles = "Pracownik")]
        public int DodajEtykiete(string nazwa)
        {
            Etykieta etykieta = new Etykieta()
            {
                Nazwa = nazwa
            };
            db.Etykiety.Add(etykieta);
            db.SaveChanges();
            SetViewBag();
            return etykieta.Id_Etykieta;
        }

        [Authorize(Roles = "Pracownik")]
        public int DodajPisarza(string imie, string nazwisko)
        {
            Pisarz pisarz = new Pisarz()
            {
                Imie = imie,
                Nazwisko = nazwisko
            };
            db.Pisarze.Add(pisarz);
            db.SaveChanges();
            SetViewBag();
            return pisarz.Id_Pisarza;
        }
    }
}