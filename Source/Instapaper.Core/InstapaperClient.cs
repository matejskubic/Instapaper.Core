using Instapaper.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Instapaper.Core
{
    public partial class InstapaperClient : IInstapaperClient, IDisposable
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private const string BaseUrl = "https://www.instapaper.com/api";
        private const string AuthUrl = BaseUrl + "/1.1/oauth";
        public readonly HttpClient HttpClient;
        public readonly HttpClientHandler HttpClientHandler;

        private InstapaperClient()
        {
            HttpClientHandler = new HttpClientHandler();
            HttpClient = new HttpClient(HttpClientHandler);
        }

        public InstapaperClient(string consumerKey, string consumerSecret, string oauthToken = null, string oauthSecret = null)
            : this()
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;

            if (!string.IsNullOrEmpty(oauthToken) && !string.IsNullOrEmpty(oauthSecret))
            {
                AccessToken = new OAuthToken(oauthToken, oauthSecret);
            }
        }

        public OAuthToken AccessToken { get; set; }

        /// <summary>
        /// Get an AccessToken for the user with the given email and password.
        /// </summary>
        /// <param name="emailAddress">The email address for the user.</param>
        /// <param name="password">The password for the user.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The AccessToken if the user is authenticated. Null if the user is not authenticated.</returns>
        public async Task<OAuthToken> GetAuthTokenAsync(string emailAddress, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            //
            // Acquire an access token
            //

            const string authUrl = AuthUrl + "/access_token";

            var parameters = new Dictionary<string, string>
            {
                { "x_auth_username", emailAddress },
                {"x_auth_password", password},
                {"x_auth_mode","client_auth"}
            };

            var oauthRequest = OAuth.OAuthRequest.ForClientAuthentication(_consumerKey, _consumerSecret, emailAddress, password);
            oauthRequest.RequestUrl = authUrl;
            oauthRequest.Method = "POST";

            var auth = oauthRequest.GetAuthorizationHeader();

            var message = new HttpRequestMessage(HttpMethod.Post, oauthRequest.RequestUrl);
            var content = new FormUrlEncodedContent(parameters);
            message.Content = content;

            message.Headers.Add("Authorization", oauthRequest.GetAuthorizationHeader());
            var response = await HttpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode == false) return null;

            var tokenBase = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var splitted = tokenBase.Split('&').Select(s => s.Split('=')).ToLookup(xs => xs[0], xs => xs[1]);
            AccessToken = new OAuthToken(splitted["oauth_token"].First(), splitted["oauth_token_secret"].First());
            return AccessToken;
        }

        private Task<HttpResponseMessage> GetResponse(string url, IDictionary<string, string> parameters , CancellationToken cancellationToken = default(CancellationToken))
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            var oauthRequest = OAuth.OAuthRequest.ForAccessToken(_consumerKey, _consumerSecret, AccessToken.Key, AccessToken.Secret);
            oauthRequest.Method = "POST";
            oauthRequest.RequestUrl = url;

            var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Headers.Add("Authorization", oauthRequest.GetAuthorizationHeader(parameters));
            message.Content = new FormUrlEncodedContent(parameters);
            return HttpClient.SendAsync(message, cancellationToken);
        }

        public async Task<InstaResponse<User>> VerifyUserAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            const string url = BaseUrl + "/1.1/account/verify_credentials";

            var response = await GetResponse(url, new Dictionary<string, string>(), cancellationToken).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = ProcessResponse<List<User>>(json);
            return result.Error == null
                ? new InstaResponse<User> { Response = result.Response.FirstOrDefault() }
                : new InstaResponse<User> { Error = result.Error };
        }

        private static InstaResponse<TReturnType> ProcessResponse<TReturnType>(string json) where TReturnType : class
        {
            if (string.IsNullOrEmpty(json))
            {
                return new InstaResponse<TReturnType> { Error = new Error { ErrorCode = 0000, Message = "API response contained no information", Type = "error" } };
            }

            if (json.Contains("error_code"))
            {
                var error = JsonConvert.DeserializeObject<List<Error>>(json);
                return new InstaResponse<TReturnType> { Error = error.FirstOrDefault() };
            }

            var response = JsonConvert.DeserializeObject<TReturnType>(json);
            return new InstaResponse<TReturnType> { Response = response };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpClientHandler.Dispose();
                    HttpClient.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
