using Dmc.Cms.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class ExternalLoginViewModel : IValidatableObject
    {
        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string ScreenName
        {
            get;
            set;
        }

        [Required]
        public string ExternalLoginProvider
        {
            get;
            set;
        }

        //[Required]
        [EmailAddress]
        public string Email
        {
            get;
            set;
        }

        [Required]
        public string Id
        {
            get;
            set;
        }

        public string ReturnUrl
        {
            get;
            set;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (!string.IsNullOrWhiteSpace(ExternalLoginProvider))
            {
                //TODO: we should have a list
                if (!ExternalLoginSettings.SupportedExternalLoginIds.Any(o => ExternalLoginProvider.Equals(o, StringComparison.Ordinal)))
                {
                    validationResults.Add(new ValidationResult("External Login Provider is not supported."));
                }
            }

            return validationResults;
        }
    }
}