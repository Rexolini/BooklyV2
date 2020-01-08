using Bookly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Bookly.App_Start
{
    public class ObslugaEmaila : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage wiadomosc)
        {
            try
            {
                //Kwalifikacje
                var kwalifikacjeUzytkownikNazwa = "smietnikkamil@gmail.com";
                var wyslaneOd = "smietnikkamil@gmail.com";
                var temat = "Bookly";

                //Konfiguracja klienta
                System.Net.Mail.SmtpClient klient = new System.Net.Mail.SmtpClient("smtp.gmail.com");

                klient.Port = 587;
                klient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                klient.UseDefaultCredentials = false;

                //Tworzenie kwalifikacji
                System.Net.NetworkCredential kwalifikacje = new System.Net.NetworkCredential(kwalifikacjeUzytkownikNazwa, temat);

                klient.EnableSsl = true;
                klient.Credentials = kwalifikacje;

                //Tworzenie wiadomosci
                var mail = new System.Net.Mail.MailMessage(wyslaneOd, wiadomosc.Destination);
                mail.Subject = wiadomosc.Subject;
                mail.Body = wiadomosc.Body;

                //Wysylanie
                return klient.SendMailAsync(mail);
            }
            catch(Exception blad)
            {
                
                throw;
            }
        }
    }

    public class ObslugaSms : IIdentityMessageService
    {
        //Twilio
        public Task SendAsync(IdentityMessage wiadomosc)
        {
            // Twilio Begin
            var accountSid = ConfigurationManager.AppSettings["123"];
            var authToken = ConfigurationManager.AppSettings["123"];
            var fromNumber = ConfigurationManager.AppSettings["Bookly"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource result = MessageResource.Create(
            new PhoneNumber(wiadomosc.Destination),
            from: new PhoneNumber(fromNumber),
            body: wiadomosc.Body
            );

            //Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status.ToString());
            //Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);    
            
        }
    }

    public class AplikacjaUzytkownikZarzadzanie : UserManager<Uzytkownik>
    {
        public AplikacjaUzytkownikZarzadzanie(IUserStore<Uzytkownik> zapas) : base(zapas)
        {
            this.UserTokenProvider = new TotpSecurityStampBasedTokenProvider<Uzytkownik, string>();
            this.EmailService = new ObslugaEmaila();
        }

        public static AplikacjaUzytkownikZarzadzanie Create(IdentityFactoryOptions<AplikacjaUzytkownikZarzadzanie> opcje, IOwinContext context)
        {
            var zarzadzanie = new AplikacjaUzytkownikZarzadzanie(new UserStore<Uzytkownik>(context.Get<AppDbContext>()));

            zarzadzanie.UserValidator = new UserValidator<Uzytkownik>(zarzadzanie)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            zarzadzanie.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false
            };

            zarzadzanie.UserLockoutEnabledByDefault = true;
            zarzadzanie.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(2);
            zarzadzanie.MaxFailedAccessAttemptsBeforeLockout = 3;

            zarzadzanie.RegisterTwoFactorProvider("Kod z Telefonu", new PhoneNumberTokenProvider<Uzytkownik>
            {
                MessageFormat = "Twoj kod dostepu to {0}"
            });

            zarzadzanie.RegisterTwoFactorProvider("Kod z Maila", new EmailTokenProvider<Uzytkownik>
            {
                Subject = "Kod Dostepu",
                BodyFormat = "Twoj kod dostepu to {0}"
            });
            zarzadzanie.EmailService = new ObslugaEmaila();
            zarzadzanie.SmsService = new ObslugaSms();

            var dataProtectionProvider = opcje.DataProtectionProvider;
            if (dataProtectionProvider != null)
                zarzadzanie.UserTokenProvider = new DataProtectorTokenProvider<Uzytkownik>(dataProtectionProvider.Create("ASP.NET Identity"));
            return zarzadzanie;
        }
    }

    public class AplikacjaLogowanieZarzadzanie : SignInManager<Uzytkownik, string>
    {
        public AplikacjaLogowanieZarzadzanie(AplikacjaUzytkownikZarzadzanie uzytkownikZarzadzanie, IAuthenticationManager uwietrzylnienieZarzadzanie)
            :base(uzytkownikZarzadzanie, uwietrzylnienieZarzadzanie){ }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Uzytkownik uzytkownik)
        {
            return uzytkownik.GenerujIdUzytkownikaAsync((AplikacjaUzytkownikZarzadzanie) UserManager);
        }

        public static AplikacjaLogowanieZarzadzanie Create(IdentityFactoryOptions<AplikacjaLogowanieZarzadzanie> opcje, IOwinContext context)
        {
            return new AplikacjaLogowanieZarzadzanie(context.GetUserManager<AplikacjaUzytkownikZarzadzanie>(), context.Authentication);
        }

        
    }
}