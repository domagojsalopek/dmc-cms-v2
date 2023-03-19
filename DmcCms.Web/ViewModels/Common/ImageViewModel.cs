using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class ImageViewModel : ICrudViewModel
    {
        public int? Id { get; set; }

        public string Name
        {
            get;
            set;
        }

        public string AltText
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string SmallImage
        {
            get;
            set;
        }

        public string LargeImage
        {
            get;
            set;
        }
    }
}