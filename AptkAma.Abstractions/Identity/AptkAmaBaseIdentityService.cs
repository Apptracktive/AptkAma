using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Aptk.Plugins.AptkAma.Identity
{
    /// <summary>
    /// Service to manage identity
    /// </summary>
    public abstract class AptkAmaBaseIdentityService : IAptkAmaIdentityService
    {
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly IMobileServiceClient _client;

        /// <summary>
        /// Service to manage identity
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="client"></param>
        protected AptkAmaBaseIdentityService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        /// <summary>
        /// The current authenticated user provided after a successful call to LoginAsync or could be provided manually
        /// </summary>
        public MobileServiceUser CurrentUser
        {
            get { return _client.CurrentUser; }
            set { _client.CurrentUser = value; }
        }

        /// <summary>
        /// Logs a user server side into Azure
        /// </summary>
        /// <param name="provider">Identity provider to log with (must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory)</param>
        /// <param name="parameters">Optional identity provider specific extra parameters</param>
        /// <param name="useSingleSignOnIfAvailable">Use single sign on if available on platform</param>
        /// <returns>An authenticated Azure user</returns>
        public abstract Task<MobileServiceUser> LoginAsync(AptkAmaAuthenticationProvider provider, IDictionary<string, string> parameters = null, bool useSingleSignOnIfAvailable = false);

        /// <summary>
        /// Logs a user client side into Azure
        /// </summary>
        /// <param name="provider">Identity provider to log with (must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory)</param>
        /// <param name="token">Identity provider authentication token</param>
        /// <returns>An authenticated Azure user</returns>
        public async Task<MobileServiceUser> LoginAsync(AptkAmaAuthenticationProvider provider, JObject token)
        {
            if (provider == AptkAmaAuthenticationProvider.None || provider == AptkAmaAuthenticationProvider.Custom)
                throw new ArgumentException("AptkAmaAuthenticationProvider must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory");

            var user = await _client.LoginAsync(provider.ToMobileServiceAuthenticationProvider(), token);

            _configuration.CacheService?.SaveCredentials(new AptkAmaCredentials(provider, user));

            return user;
        }

        /// <summary>
        /// Logs a user into Azure with login and password
        /// Sent as JObject with login and password properties
        /// Must be returned as JObject with userId and mobileServiceAuthenticationToken properties
        /// This request requires you to create a custom login api contoller
        /// </summary>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="login">User login</param>
        /// <param name="password">User password</param>
        /// <returns>An authenticated Azure user</returns>
        public async Task<MobileServiceUser> LoginAsync(string apiName, string login, string password)
        {
            var loginObject = new JObject
            {
                {nameof(login), login},
                {nameof(password), password}
            };
            
            var result = await _client.InvokeApiAsync<JObject, JObject>(apiName, loginObject);

            JToken userId, mobileServiceAuthenticationToken;

            if(!result.TryGetValue(nameof(userId), out userId))
                throw new KeyNotFoundException($"Your cutom login controleur must return a JObject with property {nameof(userId)}");

            if (!result.TryGetValue(nameof(mobileServiceAuthenticationToken), out mobileServiceAuthenticationToken))
                throw new KeyNotFoundException($"Your cutom login controleur must return a JObject with property {nameof(mobileServiceAuthenticationToken)}");

            CurrentUser = new MobileServiceUser(userId.ToObject<string>())
            {
                MobileServiceAuthenticationToken = mobileServiceAuthenticationToken.ToObject<string>()
            };
            
            _configuration.CacheService?.SaveCredentials(new AptkAmaCredentials(AptkAmaAuthenticationProvider.Custom, CurrentUser));

            return CurrentUser;
        }

        /// <summary>
        /// Logs a user into Azure with login and password
        /// Sent as JObject with login and password properties
        /// Returned as T with at list MobileServiceAuthenticationToken and UserId properties implemented from the IAptkAmaServiceUser interface
        /// This request requires you to create a custom login api contoller
        /// </summary>
        /// <typeparam name="T">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="login">User login</param>
        /// <param name="password">User password</param>
        /// <returns>An authenticated Azure user</returns>
        public async Task<Tuple<MobileServiceUser, T>> LoginAsync<T>(string apiName, string login, string password)
        {
            var loginObject = new JObject
            {
                {nameof(login), login},
                {nameof(password), password}
            };

            var result = await _client.InvokeApiAsync<JObject, T>(apiName, loginObject);
            var user = result as IAptkAmaServiceUser;
            if(user == null)
                throw new InvalidCastException($"Your {nameof(T)} class must implement the {nameof(IAptkAmaServiceUser)} interface");

            CurrentUser = new MobileServiceUser(user.UserId)
            {
                MobileServiceAuthenticationToken = user.MobileServiceAuthenticationToken
            };

            _configuration.CacheService?.SaveCredentials(new AptkAmaCredentials(AptkAmaAuthenticationProvider.Custom, CurrentUser));

            return new Tuple<MobileServiceUser, T>(CurrentUser, result);
        }

        /// <summary>
        /// Logs a user into Azure with whatever you want
        /// Sent as T with whatever property you want
        /// Returned as U with at list MobileServiceAuthenticationToken and UserId properties implemented from the IAptkAmaServiceUser interface
        /// This request requires you to create a custom login api contoller
        /// </summary>
        /// <typeparam name="T">The type of the sent instance</typeparam>
        /// <typeparam name="U">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="body">The value to be sent as the HTTP body</param>
        /// <returns>An authenticated Azure user</returns>
        public async Task<Tuple<MobileServiceUser, U>> LoginAsync<T, U>(string apiName, T body)
        {
            var result = await _client.InvokeApiAsync<T, U>(apiName, body);
            var user = result as IAptkAmaServiceUser;
            if (user == null)
                throw new InvalidCastException($"Your {nameof(U)} class must implement the {nameof(IAptkAmaServiceUser)} interface");

            CurrentUser = new MobileServiceUser(user.UserId)
            {
                MobileServiceAuthenticationToken = user.MobileServiceAuthenticationToken
            };

            _configuration.CacheService?.SaveCredentials(new AptkAmaCredentials(AptkAmaAuthenticationProvider.Custom, CurrentUser));

            return new Tuple<MobileServiceUser, U>(CurrentUser, result);
        }

        /// <summary>
        /// Register a user into Azure
        /// This request requires you to create a custom registration api contoller
        /// </summary>
        /// <typeparam name="T">The type of the sent instance</typeparam>
        /// <typeparam name="U">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="body">The value to be sent as the HTTP body</param>
        public async Task<U> RegisterAsync<T, U>(string apiName, T body)
        {
            return await _client.InvokeApiAsync<T, U>(apiName, body);
        }

        /// <summary>
        /// Check if user is logged in or silent logs in with stored credentials (if exist)
        /// </summary>
        /// <param name="controlTokenExpiration"></param>
        /// <returns>True if logged in</returns>
        public bool EnsureLoggedIn(bool controlTokenExpiration)
        {
            if (_client.CurrentUser != null)
            {
                return !controlTokenExpiration || ControlToken(_client.CurrentUser.MobileServiceAuthenticationToken);
            }

            IAptkAmaCredentials credentials;
            if (_configuration.CacheService != null && _configuration.CacheService.TryLoadCredentials(out credentials))
            {
                _client.CurrentUser = credentials.User;

                return !controlTokenExpiration || ControlToken(_client.CurrentUser.MobileServiceAuthenticationToken);
            }

            return false;
        }

        /// <summary>
        /// Refreshes access token with the identity provider for the logged in user.
        /// </summary>
        /// <returns>An authenticated Azure user</returns>
        public async Task<MobileServiceUser> RefreshUserAsync()
        {
            if (_client.CurrentUser == null && !EnsureLoggedIn(false))
                return null;

            var user = await _client.RefreshUserAsync();

            IAptkAmaCredentials credentials;
            if (_configuration.CacheService != null && _configuration.CacheService.TryLoadCredentials(out credentials))
            {
                _configuration.CacheService.SaveCredentials(new AptkAmaCredentials(credentials.Provider, user));
            }

            return user;
        }

        /// <summary>
        /// Logs a user out from Azure and clear cache (if exist)
        /// </summary>
        public async Task LogoutAsync()
        {
            await _client.LogoutAsync();
            _configuration.CacheService?.ClearCredentials();
        }

        /// <summary>
        /// Control token expiration date
        /// Code from Glenn Gailey
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if token is still valid, else false</returns>
        private static bool ControlToken(string token)
        {
            // Check for a signed-in user.
            if (string.IsNullOrEmpty(token))
            {
                // Raise an exception if there is no token.
                throw new InvalidOperationException(
                    "The client isn't signed-in or the token value isn't set.");
            }

            // Get just the JWT part of the token.
            var jwt = token.Split(new Char[] { '.' })[1];

            // Undo the URL encoding.
            jwt = jwt.Replace('-', '+');
            jwt = jwt.Replace('_', '/');
            switch (jwt.Length % 4)
            {
                case 0: break;
                case 2: jwt += "=="; break;
                case 3: jwt += "="; break;
                default:
                    throw new System.Exception(
               "The base64url string is not valid.");
            }

            // Decode the bytes from base64 and write to a JSON string.
            var bytes = Convert.FromBase64String(jwt);
            string jsonString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            // Parse as JSON object and get the exp field value, 
            // which is the expiration date as a JavaScript primative date.
            JObject jsonObj = JObject.Parse(jsonString);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // Calculate the expiration by adding the exp value (in seconds) to the 
            // base date of 1/1/1970.
            DateTime minTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var expire = minTime.AddSeconds(exp);

            // If the expiration date is less than now, the token is expired and we return true.
            return expire > DateTime.UtcNow;
        }
    }
}
