using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminPostViewModel : ContentViewModelBase
    {
        #region Constructors

        public AdminPostViewModel()
        {
            Published = DateTimeOffset.Now;
        }

        #endregion

        #region Properties

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
        public int?[] SelectedCategories
        {
            get;
            set;
        }

        public int?[] SelectedTags
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

        public List<SelectListItem> Categories
        {
            get;
            set;
        }

        public List<SelectListItem> Tags
        {
            get;
            set;
        }

        #endregion
    }
}