using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Aptk.Plugins.AptkAma.Identity
{
    /// <summary>
    /// Service to manage identity
    /// </summary>
    public interface IAptkAmaIdentityService
    {
        /// <summary>
        /// The current authenticated user provided after a successful call to LoginAsync or could be provided manually
        /// </summary>
        MobileServiceUser CurrentUser { get; set; }

        /// <summary>
        /// Logs a user server side into Azure
        /// </summary>
        /// <param name="provider">Identity provider to log with (must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory)</param>
        /// <param name="parameters">Optional identity provider specific extra parameters</param>
        /// <param name="useSingleSignOnIfAvailable">Use single sign on if available on platform</param>
        /// <returns>An authenticated Azure user</returns>
        Task<MobileServiceUser> LoginAsync(AptkAmaAuthenticationProvider provider, IDictionary<string, string> parameters = null, bool useSingleSignOnIfAvailable = false);

        /// <summary>
        /// Logs a user client side into Azure
        /// </summary>
        /// <param name="provider">Identity provider to log with (must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory)</param>
        /// <param name="token">Identity provider authentication token</param>
        /// <returns>An authenticated Azure user</returns>
        Task<MobileServiceUser> LoginAsync(AptkAmaAuthenticationProvider provider, JObject token);

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
        Task<MobileServiceUser> LoginAsync(string apiName, string login, string password);

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
        Task<Tuple<MobileServiceUser, T>> LoginAsync<T>(string apiName, string login, string password);

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
        Task<Tuple<MobileServiceUser, U>> LoginAsync<T, U>(string apiName, T body);

        /// <summary>
        /// Register a user into Azure
        /// This request requires you to create a custom registration api contoller
        /// </summary>
        /// <typeparam name="T">The type of the sent instance</typeparam>
        /// <typeparam name="U">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="body">The value to be sent as the HTTP body</param>
        Task<U> RegisterAsync<T, U>(string apiName, T body);

        /// <summary>
        /// Check if user is logged in or silent logs in with stored credentials (if exist)
        /// </summary>
        /// <returns>True if logged in</returns>
        bool EnsureLoggedIn();

        /// <summary>
        /// Logs a user out from Azure and clear cache (if exist)
        /// </summary>
        void Logout();
    }
}
