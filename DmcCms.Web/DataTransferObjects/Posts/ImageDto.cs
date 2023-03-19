using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class ImageDto
    {
        [JsonProperty(PropertyName = "thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumbnail
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "large_image", NullValueHandling = NullValueHandling.Ignore)]
        public string LargeImage
        {
            get;
            set;
        }
    }
}