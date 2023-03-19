using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminSiteResourcesViewModel : IValidatableObject
    {
        private static readonly string[] _ValidImageTypes = new string[]
        {
            "image/gif",
            "image/jpeg",
            "image/pjpeg",
            "image/png"
        };

        public string Logo
        {
            get;
            set;
        }

        public string RetinaLogo
        {
            get;
            set;
        }

        public string FooterLogo
        {
            get;
            set;
        }

        public string DefaultSiteHeader
        {
            get;
            set;
        }

        public string EmailHeader
        {
            get;
            set;
        }

        public string DefaultOpenGraphImage
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase LogoUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase RetinaLogoUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase EmailHeaderUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase FooterLogoUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase DefaultSiteHeaderUpload
        {
            get;
            set;
        }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase DefaultOpenGraphImageUpload
        {
            get;
            set;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (LogoUpload != null && LogoUpload.ContentLength > 0 && !_ValidImageTypes.Contains(LogoUpload.ContentType))
            {
                result.Add(new ValidationResult("Logo is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (RetinaLogoUpload != null && RetinaLogoUpload.ContentLength > 0 && !_ValidImageTypes.Contains(RetinaLogoUpload.ContentType))
            {
                result.Add(new ValidationResult("Retina Logo is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (EmailHeaderUpload != null && EmailHeaderUpload.ContentLength > 0 && !_ValidImageTypes.Contains(EmailHeaderUpload.ContentType))
            {
                result.Add(new ValidationResult("Email header is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (FooterLogoUpload != null && FooterLogoUpload.ContentLength > 0 && !_ValidImageTypes.Contains(FooterLogoUpload.ContentType))
            {
                result.Add(new ValidationResult("Footer Logo is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (DefaultSiteHeaderUpload != null && DefaultSiteHeaderUpload.ContentLength > 0 && !_ValidImageTypes.Contains(DefaultSiteHeaderUpload.ContentType))
            {
                result.Add(new ValidationResult("Header is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            if (DefaultOpenGraphImageUpload != null && DefaultOpenGraphImageUpload.ContentLength > 0 && !_ValidImageTypes.Contains(DefaultOpenGraphImageUpload.ContentType))
            {
                result.Add(new ValidationResult("OPen Graph image is invalid. Please choose either a GIF, JPG or PNG image."));
            }

            return result;
        }
    }
}