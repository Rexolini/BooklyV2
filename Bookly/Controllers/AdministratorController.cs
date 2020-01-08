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

namespace Bookly.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {
        private AplikacjaLogowanieZarzadzanie _logowanieZarzadzanie;
        private AplikacjaUzytkownikZarzadzanie _uzytkownikZarzadzanie;
        public AdministratorController() { }

        public AdministratorController(AplikacjaLogowanieZarzadzanie logowanieZarzadzanie, AplikacjaUzytkownikZarzadzanie uzytkownikZarzadzanie)
        {
            UzytkownikZarzadzanie = uzytkownikZarzadzanie;
            LogowanieZarzadzanie = logowanieZarzadzanie;
        }

        public AplikacjaLogowanieZarzadzanie LogowanieZarzadzanie
        {
            get
            {
                return _logowanieZarzadzanie ?? HttpContext.GetOwinContext().Get<AplikacjaLogowanieZarzadzanie>();
            }
            private set
            {
                _logowanieZarzadzanie = value;
            }
        }

        public AplikacjaUzytkownikZarzadzanie UzytkownikZarzadzanie
        {
            get
            {
                return _uzytkownikZarzadzanie ?? HttpContext.GetOwinContext().GetUserManager<AplikacjaUzytkownikZarzadzanie>();
            }
            private set
            {
                _uzytkownikZarzadzanie = value;
            }
        }

        //GET: /Zarzadzanie/Index
        public async Task<ActionResult> Index(ZarzadzanieWiadomosciaId? wiadomosc)
        {
            ViewBag.StatusWiadomosci =
                wiadomosc == ZarzadzanieWiadomosciaId.ZmianaHaslaPowodzenie ? "Twoje hasło zostało zmienione"
                : wiadomosc == ZarzadzanieWiadomosciaId.UstawienieHaslaPowodzenie ? "Twoje hasło zostało ustawione"
                : wiadomosc == ZarzadzanieWiadomosciaId.PolaczeniePowodzenie ? "Twoje podwójna autoryzacja została ustawiona"
                : wiadomosc == ZarzadzanieWiadomosciaId.Error ? "Wystąpił błąd"
                : wiadomosc == ZarzadzanieWiadomosciaId.DodanieNumeruPowodzenie ? "Twój numer telefonu został podany"
                : wiadomosc == ZarzadzanieWiadomosciaId.UsuniecieNumeruPowodzenie ? "Twoj numer telefonu został usuniety"
                : "";

            var uzytkownikId = User.Identity.GetUserId();
            var model = new ZarzadanieViewModel
            {
                PosiadaHaslo = PosiadaHaslo(),
                NumerTelefonu = await UzytkownikZarzadzanie.GetPhoneNumberAsync(uzytkownikId),
                Fuzja = await UzytkownikZarzadzanie.GetTwoFactorEnabledAsync(uzytkownikId),
                Loginy = await UzytkownikZarzadzanie.GetLoginsAsync(uzytkownikId),
                ZapamietajPrzegladarke = await WeryfikacjaZarzadzanie.TwoFactorBrowserRememberedAsync(uzytkownikId)
            };
            return View(model);
        }

          //
          // POST: /Zarzadzanie/EnableTwoFactorAuthentication
          [HttpPost, ValidateAntiForgeryToken]
          public async Task<ActionResult> EnableTwoFactorAuthentication()
          {
              await UzytkownikZarzadzanie.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
              var user = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
              if (user != null)
              {
                await LogowanieZarzadzanie.SignInAsync(user, isPersistent: false, rememberBrowser: false);
              }
              return RedirectToAction("Index", "Zarzadzanie");
          }
          //
          // POST: /Zarzadzanie/DisableTwoFactorAuthentication
          [HttpPost, ValidateAntiForgeryToken]
          public async Task<ActionResult> DisableTwoFactorAuthentication()
          {
              await UzytkownikZarzadzanie.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
              var user = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
              if (user != null)
              {
                  await LogowanieZarzadzanie.SignInAsync(user, isPersistent: false, rememberBrowser: false);
              }
              return RedirectToAction("Index", "Zarzadzanie");
          }

        //POST: /Zarzadzanie/UsunLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UsunLogin(string loginProvider, string providerKey)
        {
            ZarzadzanieWiadomosciaId? wiadomosc;
            var rezultat = await UzytkownikZarzadzanie.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if(rezultat.Succeeded)
            {
                var uzytkownik = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
                if(uzytkownik != null)
                {
                    await LogowanieZarzadzanie.SignInAsync(uzytkownik, isPersistent: false, rememberBrowser: false);
                }
                wiadomosc = ZarzadzanieWiadomosciaId.UsuniecieLoginiuPowodzenie;
            }
            else
            {
                wiadomosc = ZarzadzanieWiadomosciaId.Error;
            }
            return RedirectToAction("ZarzadzanieLoginami", new { Wiadomosc = wiadomosc });
        }

        //GET: /Zarzadzanie/DodajNumerTelefonu
        public ActionResult DodajNumerTelefonu()
        {
            return View();
        }

        //GET: /Zarzadzanie/ZmienHaslo
        public ActionResult ZmienHaslo()
        {
            return View();
        }

        //POST: /Zarzadzanie/ZmienHaslo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ZmienHaslo(ZmienHasloViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var rezultat = await UzytkownikZarzadzanie.ChangePasswordAsync(User.Identity.GetUserId(), model.StareHaslo, model.NoweHaslo);
            if(rezultat.Succeeded)
            {
                var uzytkownik = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
                if(uzytkownik != null)
                {
                    await LogowanieZarzadzanie.SignInAsync(uzytkownik, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Wiadomosc = ZarzadzanieWiadomosciaId.ZmianaHaslaPowodzenie });
            }
            AddErrors(rezultat);
            return View(model);
        }

        //GET: /Zarzadzanie/UstawHaslo
        public ActionResult UstawHaslo()
        {
            return View();
        }

        //POST: /Zarzadzanie/UstawHaslo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UstawHaslo(UstawHasloViewModel model)
        {
            if(ModelState.IsValid)
            {
                var rezultat = await UzytkownikZarzadzanie.AddPasswordAsync(User.Identity.GetUserId(), model.NoweHaslo);
                if(rezultat.Succeeded)
                {
                    var uzytkownik = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
                    if(uzytkownik != null)
                    {
                        await LogowanieZarzadzanie.SignInAsync(uzytkownik, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Wiadomosc = ZarzadzanieWiadomosciaId.UstawienieHaslaPowodzenie });
                }
                AddErrors(rezultat);
            }
            return View(model);
        }

        //GET: /Zarzadzanie/ZarzadzanieLogowaniem
        public async Task<ActionResult> ZarzadzanieLogowaniem(ZarzadzanieWiadomosciaId? wiadomosc)
        { 
                ViewBag.StatusWiadomosci =
                wiadomosc == ZarzadzanieWiadomosciaId.UsuniecieLoginiuPowodzenie ? "Twoj login zostal usuniety"
                : wiadomosc == ZarzadzanieWiadomosciaId.Error ? "Błąd"
                : "";
            var uzytkownik = await UzytkownikZarzadzanie.FindByIdAsync(User.Identity.GetUserId());
            if(uzytkownik == null)
            {
                return View("Error");
            }
            var uzytkownikLoginy = await UzytkownikZarzadzanie.GetLoginsAsync(User.Identity.GetUserId());
            var rozneLoginy = WeryfikacjaZarzadzanie.GetExternalAuthenticationTypes().Where(a
                => uzytkownikLoginy.All(u => a.AuthenticationType != u.LoginProvider)).ToList();
            ViewBag.PokazPrzyciskUsuniecia = uzytkownik.PasswordHash != null || uzytkownikLoginy.Count > 1;
            return View(new ZarzadzajLoginamiViewModel
            {
                AktualneLoginy = uzytkownikLoginy,
                InneLoginy = rozneLoginy
            });
        }

        //POST: /Zarzadzanie/LinkLogowania
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogowania(string provider)
        {
            return new KontoController.WynikWyzwania(provider, Url.Action("LinkPonownegoLogowania", "Zarzadzanie"), User.Identity.GetUserId());
        }

        //GET: /Zarzadzanie/LinkLogowaniaPonownego
        public async Task<ActionResult> LinkLogowaniaPonownego()
        {
            var loginInfo = await WeryfikacjaZarzadzanie.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if(loginInfo == null)
            {
                return RedirectToAction("ZarzadzanieLoginami", new { Wiadomosc = ZarzadzanieWiadomosciaId.Error });
            }
            var rezultat = await UzytkownikZarzadzanie.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return rezultat.Succeeded ? RedirectToAction("ZarzadzanieLoginami") :
                RedirectToAction("ZarzadzanieLoginami", new { Wiadomosc = ZarzadzanieWiadomosciaId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing && _uzytkownikZarzadzanie != null)
            {
                _uzytkownikZarzadzanie.Dispose();
                _uzytkownikZarzadzanie = null;
            }
            base.Dispose(disposing);
        }
        #region Helper
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager WeryfikacjaZarzadzanie
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult rezultat)
        {
            foreach (var error in rezultat.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool PosiadaHaslo()
        {
            var uzytkownik = UzytkownikZarzadzanie.FindById(User.Identity.GetUserId());
            if(uzytkownik != null)
            {
                return uzytkownik.PasswordHash != null;
            }
            return false;
        }

        private bool PosiadaNumerTelefonu()
        {
            var uzytkownik = UzytkownikZarzadzanie.FindById(User.Identity.GetUserId());
            if(uzytkownik != null)
            {
                return uzytkownik.PhoneNumber != null;
            }
            return false;
        }

        public enum ZarzadzanieWiadomosciaId
        {
            DodanieNumeruPowodzenie,
            ZmianaHaslaPowodzenie,
            PolaczeniePowodzenie,
            UstawienieHaslaPowodzenie,
            UsuniecieLoginiuPowodzenie,
            UsuniecieNumeruPowodzenie,
            Error
        }
        #endregion
    }
}