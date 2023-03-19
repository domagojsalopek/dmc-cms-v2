using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminCommentViewModel : CrudViewModelBase
    {
        // Generally changed by user
        public CommentStatus Status
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        // changed by admin
        public bool Approved
        {
            get;
            set;
        }
    }
}