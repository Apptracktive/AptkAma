using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using AptkAma.Sample.Backend.DataObjects;
using AptkAma.Sample.Backend.Models;
using AptkAma.Sample.Backend.Utils;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Login;
using Newtonsoft.Json.Linq;

namespace AptkAma.Sample.Backend.Controllers
{
    [MobileAppController]
    public class CustomLoginController : ApiController
    {
        private readonly string _host = $"https://{Environment.ExpandEnvironmentVariables("%WEBSITE_SITE_NAME%").ToLower()}.azurewebsites.net/";

        public HttpResponseMessage Post(LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Login) || string.IsNullOrEmpty(loginRequest.Password))
                return Request.CreateBadRequestResponse("Login and Password should not be null");

            var context = new MobileServiceContext();
            var account = context.Accounts.SingleOrDefault(a => a.Login == loginRequest.Login);
            if (account != null)
            {
                var incoming = CustomLoginProviderUtils.Hash(loginRequest.Password, account.Salt);

                if (CustomLoginProviderUtils.SlowEquals(incoming, account.SaltedAndHashedPassword))
                {
                    var token = AppServiceLoginHandler.CreateToken(new[] { new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Login) },
                    GetSigningKey(),
                    _host,
                    _host,
                    TimeSpan.FromHours(24));
                    var customLoginResult = new JObject
                    {
                        { "userId", account.Id },
                        { "mobileServiceAuthenticationToken", token.RawData }
                    };
                    return this.Request.CreateResponse(HttpStatusCode.OK, customLoginResult);
                }
            }
            return this.Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
        }

        private static string GetSigningKey()
        {
            string key =
                Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");

            if (string.IsNullOrWhiteSpace(key))
                key = ConfigurationManager.AppSettings["SigningKey"];

            return key;
        }
    }
}
