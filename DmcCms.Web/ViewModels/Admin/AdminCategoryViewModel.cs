using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminCategoryViewModel : ContentViewModelBase
    {
        public AdminCategoryViewModel()
        {
            Published = DateTimeOffset.Now;
        }

        public int? ParentId
        {
            get;
            set;
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

        public List<SelectListItem> Categories
        {
            get;
            set;
        }

        public List<SelectListItem> Images
        {
            get;
            internal set;
        }

        public int? IntroImageId { get; set; }
    }
}