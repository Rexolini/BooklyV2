using Bookly.App_Start;
using Bookly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;



namespace Bookly.Controllers
{
    public class KontoController : Controller
    {
         
        
            private AppDbContext dbContext = new AppDbContext();
            private AplikacjaLogowanieZarzadzanie _logowanieZarzadzanie;
            private AplikacjaUzytkownikZarzadzanie _uzytkownikZarzadzanie;

            public KontoController() { }

            public KontoController(AplikacjaUzytkownikZarzadzanie uzytkownikZarzadzanie, AplikacjaLogowanieZarzadzanie logowanieZarzadzanie)
            {
                UzytkownikZarzadzanie = uzytkownikZarzadzanie;
                LogowanieZarzadzanie = logowanieZarzadzanie;
            }

            public AplikacjaLogowanieZarzadzanie LogowanieZarzadzanie
            {
                get { return _logowanieZarzadzanie ?? HttpContext.GetOwinContext().Get<AplikacjaLogowanieZarzadzanie>(); }
                private set { _logowanieZarzadzanie = value; }
            }

            public AplikacjaUzytkownikZarzadzanie UzytkownikZarzadzanie
            {
                get { return _uzytkownikZarzadzanie ?? HttpContext.GetOwinContext().Get<AplikacjaUzytkownikZarzadzanie>(); }
                private set { _uzytkownikZarzadzanie = value; }
            }


            // GET: /Konto/Logowanie
            [AllowAnonymous]
            public ActionResult Logowanie(string returnUrl)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            //POST: /Konto/Logowanie
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Logowanie(LogowanieViewModel model, string returnUrl)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var rezultat = await LogowanieZarzadzanie.PasswordSignInAsync(model.Login, model.Haslo, model.ZapamietajMnie, shouldLockout: false);
                    if (rezultat == SignInStatus.Success)
                    {
                        var uzytkownik = dbContext.Users.FirstOrDefault(x => x.UserName == model.Login);
                        if (UzytkownikZarzadzanie.IsEmailConfirmedAsync(uzytkownik.Id).Result == false)
                        {
                            rezultat = SignInStatus.LockedOut;
                            AutoryzacjaZarzadzanie.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        }
                    }
                    switch (rezultat)
                    {
                        case SignInStatus.Success:
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("WyslijKod", new { ReturnUrl = returnUrl, ZapamietajMnie = model.ZapamietajMnie });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login Attempt.");
                            return View(model);
                    }
                
            }

            //GET: /konto/Weryfikacja
            [AllowAnonymous]
            public async Task<ActionResult> Weryfikacja(string provider, string returnUrl, bool zapamietajMnie)
            {
                if (!await LogowanieZarzadzanie.HasBeenVerifiedAsync())
                {
                    return View("Error");
                }
                return View(new WeryfikacjaKoduViewModel { Provider = provider, ZwrocUrl = returnUrl, ZapamietajMnie = zapamietajMnie });
            }

            //POST: /Konto/Weryfikacja
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Weryfikacja(WeryfikacjaKoduViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var rezultat = await LogowanieZarzadzanie.TwoFactorSignInAsync(model.Provider, model.Kod,
                    model.ZapamietajMnie, model.ZapamietajPrzegladarke);
                switch (rezultat)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(model.ZwrocUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Błędny kod");
                        return View(model);
                }
            }

            //GET: /Konto/Rejestracja
            [AllowAnonymous]
            public ActionResult Rejestracja()
            {
                return View();
            }

            //POST: /Konto/Rejestracja
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Rejestracja(RejestracjaViewModel model)
            {
                if (ModelState.IsValid)
                {
                var uzytkownik = new Uzytkownik { UserName = model.Login, Email = model.Email, PasswordHash = model.Haslo, Imie = model.Imie, Nazwisko = model.Nazwisko };
                var rezultat = await UzytkownikZarzadzanie.CreateAsync(uzytkownik, model.Haslo);
                    if (rezultat.Succeeded)
                    {
                        return View("Potwierz Maila");
                    }
                    AddError(rezultat);
                }
                return View(model);
            }

        //GET: /Konto/PotwierdzenieMaila
        [AllowAnonymous]
        public async Task<ActionResult> PotwierdzenieMaila(string uzytkownikId, string kod)
        {
            object p = null;
            if (uzytkownikId == p || kod == p)
            {
                return View("Błąd");
            }
            var rezultat = await UzytkownikZarzadzanie.ConfirmEmailAsync(uzytkownikId, kod);
            return View(rezultat.Succeeded ? "Potwierdz Mail" : "Błąd");
        }

            //GET: /Konto/ZapomnianeHaslo
            [AllowAnonymous]
            public ActionResult ZapomnianeHaslo()
            {
                return View();
            }

            //POST: /Konto/ZapomnianeHaslo
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ZapomnianeHaslo(ZapomnialemHaslaViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var uzytkownik = await UzytkownikZarzadzanie.FindByEmailAsync(model.Email);
                    if (uzytkownik == null || !(await UzytkownikZarzadzanie.IsEmailConfirmedAsync(uzytkownik.Id)))
                    {
                        return View("ZapomnianeHasloPotwierdzenie");
                    }

                    string kod = await UzytkownikZarzadzanie.GeneratePasswordResetTokenAsync(uzytkownik.Id);
                    var przywrocUrl = Url.Action("ResetujHaslo", "Konto", new { userId = uzytkownik.Id, code = kod }, protocol: Request.Url.Scheme);
                    await UzytkownikZarzadzanie.SendEmailAsync(uzytkownik.Id, "Resetuj Haslo", "Zresetuj haslo klikajac w link: " + przywrocUrl + Environment.NewLine);
                    return RedirectToAction("ZapomnijHasloPotwierdzenie", "Konto");
                }
                return View(model);
            }

            //GET: /Konto/ZapomnijHasloPotiwerdzenie
            [AllowAnonymous]
            public ActionResult ZapomnijHasloPotwierdzenie()
            {
                return View();
            }

            //GET: /Konto/ZresetujHaslo
            [AllowAnonymous]
            public ActionResult ZresetujHaslo(string Kod)
            {
                return Kod == null ? View("Błąd") : View();
            }

            //POST: /Konto/ZresetujHaslo
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ZresetujHaslo(ResetujHasloViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var uzytkownik = await UzytkownikZarzadzanie.FindByEmailAsync(model.Email);
                if (uzytkownik == null)
                {
                    return RedirectToAction("ZresetujHasloPotiwerdzenie", "Konto");
                }
                var rezultat = await UzytkownikZarzadzanie.ResetPasswordAsync(uzytkownik.Id, model.Kod, model.Haslo);
                if (rezultat.Succeeded) ;
                return View();
            }

            //GET: /Konto/ZresetujHasloPotiwerdzenie
            [AllowAnonymous]
            public ActionResult ZresetujHasloPotiwerdzenie()
            {
                return View();
            }
            //POST: /Konto/ZewnetrznyLogin
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public ActionResult ZewnetrznyLogin(string provider, string zwrocUrl)
            {
                return new WynikWyzwania(provider, Url.Action("ZewnetrznyLogin", "Konto", new { ReturnUrl = zwrocUrl }));
            }

            //GET: /Konto/WyslijKod
            [AllowAnonymous]
            public async Task<ActionResult> WyslijKod(string zwrocUrl, bool zapamietajMnie)
            {
                var uzytkownikId = await LogowanieZarzadzanie.GetVerifiedUserIdAsync();
                if (uzytkownikId == null)
                {
                    return View("Błąd");
                }
                var uzytkownikFactor = await UzytkownikZarzadzanie.GetValidTwoFactorProvidersAsync(uzytkownikId);
                var factorOptions = uzytkownikFactor.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
                return View(new WyslijKodViewModel { Providers = factorOptions, ZwrocUrl = zwrocUrl, ZapamietajMnie = zapamietajMnie });
            }

            //POST: /Konto/WyslijKod
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> WyslijKod(WyslijKodViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!await LogowanieZarzadzanie.SendTwoFactorCodeAsync(model.SelectedProvider))
                {
                    return View("Błąd");
                }
                return RedirectToAction("KodWeryfikujacy", new { Provider = model.SelectedProvider, ReturnUrl = model.ZwrocUrl, RememberMe = model.ZapamietajMnie });
            }

            //GET: /Konto/ZewnetrznyLoginPowrot
            [AllowAnonymous]
            public async Task<ActionResult> ZewnetrznyLoginPowrot(string zwrocUrl)
            {
                var loginInfo = await AutoryzacjaZarzadzanie.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return RedirectToAction("Login");
                }

                var rezultat = await LogowanieZarzadzanie.ExternalSignInAsync(loginInfo, isPersistent: false);
                switch (rezultat)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(zwrocUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("WyslijKod", new { ReturnUrl = zwrocUrl, RememberMe = false });
                    case SignInStatus.Failure:
                    default:
                        ViewBag.ReturnUrl = zwrocUrl;
                        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                        return View("ZewnetrznyLoginPotwierdzenie", new ZewnetrznePotwierdzenieLogowaniaViewModel { Email = loginInfo.Email });
                }
            }

            //POST: /Konto/ZewnetrznePotwierdzenieLogowania
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ZewnetrznePotwierdzenieLogowania(ZewnetrznePotwierdzenieLogowaniaViewModel model, string zwrocUrl)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Zarzadzaj");
                }

                if (ModelState.IsValid)
                {
                    var info = await AutoryzacjaZarzadzanie.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                    return View("ZewnetrzneLogowanieNiepowodzenie");
                    }
                    var uzytkownik = new Uzytkownik { UserName = model.Email, Email = model.Email };
                    var rezultat = await UzytkownikZarzadzanie.CreateAsync(uzytkownik);
                    if (rezultat.Succeeded)
                    {
                        rezultat = await UzytkownikZarzadzanie.AddLoginAsync(uzytkownik.Id, info.Login);
                        if (rezultat.Succeeded)
                        {
                            await LogowanieZarzadzanie.SignInAsync(uzytkownik, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(zwrocUrl);

                        }
                    }
                    AddError(rezultat);
                }
                ViewBag.ReturnUrl = zwrocUrl;
                return View(model);
            }

            //POST: /Konto/Wyloguj
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Wyloguj()
            {
                AutoryzacjaZarzadzanie.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                if (Session["koszyk"] != null)
                {
                    Session["koszyk"] = null;
                }
                return RedirectToAction("Index", "Home");

            }

            //GET: /Konto/ZewnetrznyLoginNiepowodzenie
            [AllowAnonymous]
            public ActionResult ZewnetrznyLoginNiepowodzenie()
            {
                return View();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_uzytkownikZarzadzanie != null)
                    {
                        _uzytkownikZarzadzanie.Dispose();
                        _uzytkownikZarzadzanie = null;
                    }

                    if (_logowanieZarzadzanie != null)
                    {
                        _logowanieZarzadzanie.Dispose();
                        _logowanieZarzadzanie = null;
                    }
                }
                base.Dispose(disposing);
            }

            #region Helper

            private const string Key = "Key";
            private IAuthenticationManager AutoryzacjaZarzadzanie
            {
                get
                {
                    return HttpContext.GetOwinContext().Authentication;
                }
            }

            private void AddError(IdentityResult rezultat)
            {
                foreach (var error in rezultat.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            private ActionResult RedirectToLocal(string returnUrl)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            internal class WynikWyzwania : HttpUnauthorizedResult
            {
                public WynikWyzwania(string provider, string redirectUri) : this(provider, redirectUri, null)
                { }

                public WynikWyzwania(string provider, string redirectUri, string uzytkownikId)
                {
                    LoginProvider = provider;
                    RedirectUri = redirectUri;
                    UzytkownikId = uzytkownikId;
                }
                public string LoginProvider { get; set; }
                public string RedirectUri { get; set; }
                public string UzytkownikId { get; set; }

                public override void ExecuteResult(ControllerContext context)
                {
                    var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                    if (UzytkownikId != null)
                    {
                        properties.Dictionary[Key] = UzytkownikId;

                    }
                    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
                }
            }
            #endregion

        
    }
}