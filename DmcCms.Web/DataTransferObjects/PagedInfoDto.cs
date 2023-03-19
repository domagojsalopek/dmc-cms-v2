using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class PagedInfoDto
    {
        [JsonProperty(PropertyName = "page", NullValueHandling = NullValueHandling.Ignore)]
        public int Page
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "per_page", NullValueHandling = NullValueHandling.Ignore)]
        public int PerPage
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "total_count", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalCount
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "total_pages", NullValueHandling = NullValueHandling.Ignore)]
        public int TotalPages
        {
            get;
            set;
        }
    }
}