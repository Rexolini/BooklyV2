using Bookly.App_Start;
using Bookly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookly
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<AplikacjaUzytkownikZarzadzanie>(AplikacjaUzytkownikZarzadzanie.Create);
            app.CreatePerOwinContext<AplikacjaLogowanieZarzadzanie>(AplikacjaLogowanieZarzadzanie.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/Konto/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AplikacjaUzytkownikZarzadzanie, Uzytkownik>(
                        validateInterval: TimeSpan.FromMinutes(15),
                        regenerateIdentity: (administrator, uzytkownik) => uzytkownik.GenerujIdUzytkownikaAsync(administrator))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}