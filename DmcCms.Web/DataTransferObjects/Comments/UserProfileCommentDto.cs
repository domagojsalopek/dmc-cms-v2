using Dmc.Cms.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class UserProfileCommentDto
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id
        {
            get;
            set;
        }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string Author
        {
            get;
            set;
        }

        [JsonProperty("post_slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug
        {
            get;
            set;
        }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CommentStatus Status
        {
            get;
            set;
        }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text
        {
            get;
            set;
        }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Date
        {
            get;
            set;
        }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public UserProfileCommentDto Parent
        {
            get;
            set;
        }

        [JsonProperty("is_own", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOwn
        {
            get;
            set;
        }

        [JsonProperty("is_reply", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsReply => Parent != null;
    }
}