using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class PostDto
    {
        #region Constructors

        public PostDto()
        {
            Tags = new List<TagDto>();
            Categories = new List<CategoryDto>();
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore)]
        public string Link
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "preview_image", NullValueHandling = NullValueHandling.Ignore)]
        public ImageDto PreviewImage
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Created
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "updated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Updated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<TagDto> Tags
        {
            get;
            private set;
        }

        [JsonProperty(PropertyName = "categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<CategoryDto> Categories
        {
            get;
            private set;
        }

        #endregion
    }
}