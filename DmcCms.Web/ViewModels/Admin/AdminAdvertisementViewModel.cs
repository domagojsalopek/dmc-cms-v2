using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminAdvertisementViewModel : CrudViewModelBase, IValidatableObject
    {
        public AdminAdvertisementViewModel()
        {
            AdvertisementType = AdvertisementType.Aside;
        }

        public Guid UniqueId
        {
            get;
            set;
        }

        public AdvertisementType AdvertisementType
        {
            get;
            set;
        }

        [Required]
        public string Name
        {
            get;
            set;
        }

        [Required]
        [AllowHtml]
        public string Html
        {
            get;
            set;
        }

        public bool IsVisible
        {
            get;
            set;
        }

        public DateTimeOffset? VisibleFrom
        {
            get;
            set;
        }

        public DateTimeOffset? VisibleTo
        {
            get;
            set;
        }

        #region IValidatableObject

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (VisibleFrom.HasValue && VisibleTo.HasValue)
            {
                if (VisibleTo.Value < VisibleFrom.Value)
                {
                    result.Add(new ValidationResult("Visible To Date cannot be smaller than Visible From."));
                }
            }

            return result;
        }

        #endregion
    }
}