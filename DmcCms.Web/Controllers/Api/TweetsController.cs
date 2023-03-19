using Dmc.Cms.App;
using Dmc.Cms.App.Services;
using Dmc.Cms.Web.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Dmc.Cms.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api")]
    public class TweetsController : ApiController
    {
        #region Fields

        private readonly IAppConfig _AppConfiguration;
        private readonly ITweetsService _TwitterService;

        #endregion

        #region Constructors

        public TweetsController() 
            : this(AppConfig.Instance)
        {

        }

        public TweetsController(IAppConfig appConfiguration) 
            : this(new TwitterService("", "")) // not used currently anyway as hosting does not allow it. todo detect,
        {
            _AppConfiguration = appConfiguration;
        }

        public TweetsController(ITweetsService tweetsService)
        {
            _TwitterService = tweetsService ?? throw new ArgumentNullException(nameof(tweetsService));
        }

        #endregion

        #region Api Methods

        [HttpGet]
        [Route("v1.0/tweets")]
        public async Task<HttpResponseMessage> GetTweets([FromUri] string username,  [FromUri] int? count)
        {
            // we don't cache because service is caching
            var tweets = await _TwitterService.GetLatestTweetsAsync(username, count ?? 3);
            if (tweets == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error.");
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(tweets)
                    , Encoding.UTF8
                    , "application/json"
                ),
                ReasonPhrase = HttpStatusCode.OK.ToString(),
            };
        }

        #endregion
    }
}
