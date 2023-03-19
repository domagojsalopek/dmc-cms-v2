using Dmc.Cms.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class CommentDto
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id
        {
            get;
            set;
        }

        [JsonProperty("parent_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? ParentId
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

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public string Author
        {
            get;
            set;
        }

        [JsonProperty("parent_author", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentAuthor
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

        [JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
        public string Website
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

        [JsonProperty("replies", NullValueHandling = NullValueHandling.Ignore)]
        public List<CommentDto> Replies
        {
            get;
            set;
        }
    }
}