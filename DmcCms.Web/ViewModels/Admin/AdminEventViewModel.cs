using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminEventViewModel : ContentViewModelBase
    {
        #region Constructors

        public AdminEventViewModel()
        {
            Published = DateTimeOffset.Now;
            EventDate = DateTime.Today;
            Order = default(int);
            EventType = EventType.Occurence;
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

        public int? ImageId
        {
            get;
            set;
        }

        [Required]
        public EventType EventType
        {
            get;
            set;
        }

        [Required]
        public DateTime EventDate
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

        #endregion

        #region Select

        public List<SelectListItem> Images
        {
            get;
            set;
        }

        #endregion
    }
}