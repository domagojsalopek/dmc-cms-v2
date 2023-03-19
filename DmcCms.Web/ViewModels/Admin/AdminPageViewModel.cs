using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminPageViewModel : ContentViewModelBase
    {
        public AdminPageViewModel()
        {
            Order = default(int);
            Published = DateTimeOffset.Now;
        }

        [Required]
        public string Title
        {
            get;
            set;
        }

        [AllowHtml]
        public string Description
        {
            get;
            set;
        }

        [Required]
        [AllowHtml]
        public string Content
        {
            get;
            set;
        }

        [Required]
        public int? Order
        {
            get;
            set;
        }

        public int? PreviewImageId
        {
            get;
            set;
        }

        public int? DetailImageId
        {
            get;
            set;
        }

        public List<SelectListItem> Images
        {
            get;
            set;
        }
    }
}