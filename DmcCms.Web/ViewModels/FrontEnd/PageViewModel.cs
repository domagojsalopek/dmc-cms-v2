using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class PageViewModel : ContentViewModelBase
    {
        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public ImageViewModel PreviewImage
        {
            get;
            set;
        }

        public ImageViewModel DetailImage
        {
            get;
            set;
        }
    }
}