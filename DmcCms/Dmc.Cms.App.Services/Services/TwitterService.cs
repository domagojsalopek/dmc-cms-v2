using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Services
{
    public class TwitterService : ITweetsService
    {
        private const string CacheKeyFormat = "Tweets_{0}_{1}";
        private const string TokenCacheKey = "TwitterOauthToken";
        private static readonly TimeSpan _CacheExpiration = TimeSpan.FromSeconds(120);

        public TwitterService(string apiKey, string secret)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ApiSecret = secret ?? throw new ArgumentNullException(nameof(secret));
            HttpClient = new HttpClient();
        }

        private string ApiKey { get; }
        private string ApiSecret { get; }
        private HttpClient HttpClient { get; }

        public async Task<dynamic> GetLatestTweetsAsync(string twitterUserName, int numberOfTweets)
        {
            string cacheKey = string.Format(CacheKeyFormat, twitterUserName, numberOfTweets);
            dynamic fromCache = CacheHelper.GetFromCache<dynamic>(cacheKey);

            if (fromCache != null)
            {
                return fromCache;
            }

            var result = await TryGetTweets(twitterUserName, numberOfTweets);

            if (result != null)
            {
                CacheHelper.AddToCache(cacheKey, result, DateTimeOffset.UtcNow.Add(_CacheExpiration));
            }

            return result;
        }

        private async Task<dynamic> TryGetTweets(string twitterUserName, int numberOfTweets)
        {
            string token = await GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&exclude_replies=1", numberOfTweets, twitterUserName));
            requestUserTimeline.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage responseUserTimeLine = await HttpClient.SendAsync(requestUserTimeline);
            if (!responseUserTimeLine.IsSuccessStatusCode)
            {
                return null;
            }

            string responseAsString = await responseUserTimeLine.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseAsString))
            {
                return null;
            }

            try
            {
                dynamic json = JsonConvert.DeserializeObject<object>(responseAsString);
                return json;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Private Methods

        private async Task<string> GetToken()
        {
            string tokenFromCache = CacheHelper.GetFromCache<string>(TokenCacheKey);
            if (!string.IsNullOrWhiteSpace(tokenFromCache))
            {
                return tokenFromCache;
            }

            try
            {
                var customerInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(ApiKey + ":" + ApiSecret));
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token");

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", customerInfo);
                request.Content = new StringContent("grant_type=client_credentials"
                    , Encoding.UTF8
                    , "application/x-www-form-urlencoded"
                );

                HttpResponseMessage response = await HttpClient.SendAsync(request);

                string json = await response.Content.ReadAsStringAsync();
                dynamic item = JsonConvert.DeserializeObject<object>(json);
                string token = item["access_token"];

                if (!string.IsNullOrWhiteSpace(token))
                {
                    CacheHelper.AddToCache(TokenCacheKey, token); // never expired
                }

                return token;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
