using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class CategoryViewModel : ContentViewModelBase
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

        public ImageViewModel IntroImage
        {
            get;
            set;
        }

        public EntityList<PostViewModel> Posts
        {
            get;
            set;
        }
    }
}