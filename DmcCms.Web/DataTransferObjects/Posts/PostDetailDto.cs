using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class PostDetailDto : PostDto
    {
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Content
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "detail_image", NullValueHandling = NullValueHandling.Ignore)]
        public ImageDto DetailImage
        {
            get;
            set;
        }
    }
}