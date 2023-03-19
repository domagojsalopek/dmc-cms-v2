using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class PostInfoViewModel
    {
        public int Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Slug
        {
            get;
            set;
        }
    }
}