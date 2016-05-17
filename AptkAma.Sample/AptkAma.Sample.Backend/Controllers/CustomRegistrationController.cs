using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using AptkAma.Sample.Backend.DataObjects;
using AptkAma.Sample.Backend.Models;
using AptkAma.Sample.Backend.Utils;
using Microsoft.Azure.Mobile.Server.Config;

namespace AptkAma.Sample.Backend.Controllers
{
    [MobileAppController]
    public class CustomRegistrationController : ApiController
    {
        // POST api/CustomRegistration
        public async Task<HttpResponseMessage> Post(RegistrationRequest registrationRequest)
        {
            if (!Regex.IsMatch(registrationRequest.Login, "^[a-zA-Z0-9]{4,}$"))
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid login (at least 4 chars, alphanumeric only)");
            }
            if (registrationRequest.Password.Length < 8)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid password (at least 8 chars required)");
            }

            var context = new AptkAmaContext();
            var account = context.Accounts.SingleOrDefault(a => a.Login == registrationRequest.Login);
            if (account != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "That login already exists.");
            }

            byte[] salt = CustomLoginProviderUtils.GenerateSalt();
            var newAccount = new Account
            {
                Id = Guid.NewGuid().ToString(),
                Name = registrationRequest.Name,
                Login = registrationRequest.Login,
                Salt = salt,
                SaltedAndHashedPassword = CustomLoginProviderUtils.Hash(registrationRequest.Password, salt)
            };
            context.Accounts.Add(newAccount);
            await context.SaveChangesAsync();
            return this.Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}
