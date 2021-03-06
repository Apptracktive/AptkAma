﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Api
{
    /// <summary>
    /// Service to send custom request
    /// </summary>
    public class AptkAmaApiService : IAptkAmaApiService
    {
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly IMobileServiceClient _client;

        /// <summary>
        /// Service to send custom request
        /// </summary>
        /// <param name="configuration">Main plugin configuration</param>
        /// <param name="client">Mobile service client instance</param>
        public AptkAmaApiService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        /// <summary>
        /// Invokes a user-defined custom API of an Azure Mobile Apps application using an HTTP POST
        /// </summary>
        /// <typeparam name="T">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <returns></returns>
        public async Task<T> InvokeApiAsync<T>(string apiName)
        {
            return await _client.InvokeApiAsync<T>(apiName);
        }

        /// <summary>
        /// Invokes a user-defined custom API of an Azure Mobile Apps application using an HTTP POST with support for sending HTTP content
        /// </summary>
        /// <typeparam name="T">The type of the sent instance</typeparam>
        /// <typeparam name="U">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="body">The value to be sent as the HTTP body</param>
        /// <returns></returns>
        public async Task<U> InvokeApiAsync<T, U>(string apiName, T body)
        {
            return await _client.InvokeApiAsync<T, U>(apiName, body);
        }

        /// <summary>
        /// Invokes a user-defined custom API of an Azure Mobile Apps application using the specifed HTTP method with support of additional parameters
        /// </summary>
        /// <typeparam name="T">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="method">The HTTP method</param>
        /// <param name="parameters">A dictionary of user-defined parameters and values to include in the request</param>
        /// <returns></returns>
        public async Task<T> InvokeApiAsync<T>(string apiName, HttpMethod method, IDictionary<string, string> parameters)
        {
            return await _client.InvokeApiAsync<T>(apiName, method, parameters);
        }

        /// <summary>
        /// Invokes a user-defined custom API of an Azure Mobile Apps application using the specifed HTTP method with support of HTTP content and additional parameters
        /// </summary>
        /// <typeparam name="T">The type of the sent instance</typeparam>
        /// <typeparam name="U">The type of the returned instance</typeparam>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="body">The value to be sent as the HTTP body</param>
        /// <param name="method">The HTTP method</param>
        /// <param name="parameters">A dictionary of user-defined parameters and values to include in the request</param>
        /// <returns></returns>
        public async Task<U> InvokeApiAsync<T, U>(string apiName, T body, HttpMethod method, IDictionary<string, string> parameters)
        {
            return await _client.InvokeApiAsync<T, U>(apiName, body, method, parameters);
        }

        /// <summary>
        /// Invokes a user-defined custom API of an Azure Mobile Apps application using the specifed HTTP method with support of HTTP content and additional parameters
        /// </summary>
        /// <param name="apiName">The name of the custom API</param>
        /// <param name="content"></param>
        /// <param name="method">The HTTP method</param>
        /// <param name="requestHeaders"></param>
        /// <param name="parameters">A dictionary of user-defined parameters and values to include in the request</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> InvokeApiAsync(string apiName, HttpContent content, HttpMethod method, IDictionary<string, string> requestHeaders,
            IDictionary<string, string> parameters)
        {
            return await _client.InvokeApiAsync(apiName, content, method, requestHeaders, parameters);
        }
    }
}
