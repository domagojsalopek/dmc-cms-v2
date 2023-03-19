using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminImageViewModel : ICrudViewModel, IValidatableObject
    {
        private static readonly string[] _ValidImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/pjpeg",
            "image/png"
        };

        #region Properties

        public int? Id
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
        public string AltText
        {
            get;
            set;
        }

        [Required]
        [AllowHtml]
        public string Caption
        {
            get;
            set;
        }

        public string SmallImageUploadPath
        {
            get;
            set;
        }

        public string LargeImageUploadPath
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase SmallImageUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase LargeImageUpload
        {
            get;
            set;
        }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (!IsSmallImageSet())
            {
                result.Add(new ValidationResult("Small Image Is Required."));
            }

            if (!IsLargeImageSet())
            {
                result.Add(new ValidationResult("Large Image Is Required."));
            }

            if (SmallImageUpload != null && SmallImageUpload.ContentLength > 0 && !_ValidImageTypes.Contains(SmallImageUpload.ContentType))
            {
                result.Add(new ValidationResult("Small Image is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (LargeImageUpload != null && LargeImageUpload.ContentLength > 0 && !_ValidImageTypes.Contains(LargeImageUpload.ContentType))
            {
                result.Add(new ValidationResult("Large Image is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            return result;
        }

        private bool IsSmallImageSet()
        {
            if (SmallImageUpload != null && SmallImageUpload.ContentLength > 0)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(SmallImageUploadPath))
            {
                return true;
            }

            return false;
        }

        private bool IsLargeImageSet()
        {
            if (LargeImageUpload != null && LargeImageUpload.ContentLength > 0)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(LargeImageUploadPath))
            {
                return true;
            }

            return false;
        }
    }
}