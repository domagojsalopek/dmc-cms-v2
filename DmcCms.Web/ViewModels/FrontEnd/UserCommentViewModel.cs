using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class UserCommentViewModel
    {
        public int CommentId
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string PostSlug
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public int? ReplyToId
        {
            get;
            set;
        }

        public string ReplyToAuthor
        {
            get;
            set;
        }

        public string ReplyToText
        {
            get;
            set;
        }

        public DateTimeOffset DateCreated
        {
            get;
            set;
        }

        public bool IsReply => ReplyToId.HasValue;
    }
}