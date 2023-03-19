using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class ApiResult
    {
        public ApiResult()
        {
            Messages = new List<string>();
        }

        [JsonProperty(PropertyName = "code", NullValueHandling = NullValueHandling.Ignore)]
        public string Code
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "messages", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Messages
        {
            get;
            private set;
        }
    }

    public class ApiResult<T> : ApiResult
    {
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public T Result
        {
            get;
            set;
        }
    }
}