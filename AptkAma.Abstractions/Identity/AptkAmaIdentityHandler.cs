using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Identity
{
    /// <summary>
    /// DelegatingHandler to automaticaly login user again if its auth token expired
    /// </summary>
    public class AptkAmaIdentityHandler : DelegatingHandler
    {
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly Action _onLoggedOut;
        private IAptkAmaCredentials _credentials;
        public IAptkAmaService AptkAmaService;

        public AptkAmaIdentityHandler(IAptkAmaPluginConfiguration configuration, Action onLoggedOut = null)
        {
            _configuration = configuration;
            _onLoggedOut = onLoggedOut;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Resolve IMvxAmsService if not yet defined
                if (AptkAmaService == null)
                {
                    throw new InvalidOperationException("Make sure to configure AptkAma plugin properly before using it.");
                }

                // Cloning the request
                var clonedRequest = await CloneRequest(request);

                // Load saved user if possible
                if (_configuration.CacheService != null
                    && _configuration.CacheService.TryLoadCredentials(out _credentials)
                    && (AptkAmaService.Identity.CurrentUser == null
                    || (AptkAmaService.Identity.CurrentUser.UserId != _credentials.User.UserId
                    && AptkAmaService.Identity.CurrentUser.MobileServiceAuthenticationToken != _credentials.User.MobileServiceAuthenticationToken)))
                {
                    AptkAmaService.Identity.CurrentUser = _credentials.User;

                    clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                    // Set the authentication header
                    clonedRequest.Headers.Add("X-ZUMO-AUTH", _credentials.User.MobileServiceAuthenticationToken);

                    // Resend the request
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized
                    && _credentials != null
                    && _credentials.Provider != AptkAmaAuthenticationProvider.None
                    && _credentials.Provider != AptkAmaAuthenticationProvider.Custom)
                {
                    try
                    {
                        // Login user again
                        var user = await AptkAmaService.Identity.LoginAsync(_credentials.Provider);

                        // Save the user if possible
                        if (_credentials == null) _credentials = new AptkAmaCredentials(_credentials.Provider, user);
                        _configuration.CacheService?.SaveCredentials(_credentials);

                        clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                        // Set the authentication header
                        clonedRequest.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                        // Resend the request
                        response = await base.SendAsync(clonedRequest, cancellationToken);
                    }
                    catch (InvalidOperationException)
                    {
                        // user cancelled auth, so lets return the original response
                        return response;
                    }
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _onLoggedOut?.Invoke();
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }
    }
}
