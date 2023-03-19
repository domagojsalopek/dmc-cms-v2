using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ITweetsService
    {
        Task<dynamic> GetLatestTweetsAsync(string twitterUserName, int numberOfTweets);
    }
}
