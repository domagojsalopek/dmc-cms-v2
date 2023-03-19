using Dmc.Cms.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminExternalLoginSettingsViewModel : IValidatableObject
    {
        public AdminExternalLoginSettingsViewModel()
        {
            Settings = new List<AdminExternalLoginSettingViewModel>();
        }

        public List<AdminExternalLoginSettingViewModel> Settings { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            foreach (var item in Settings)
            {
                if (!ExternalLoginSettings.SupportedExternalLoginIds.Any(o => item.ServiceIdentifier.Equals(o, StringComparison.Ordinal)))
                {
                    validationResults.Add(new ValidationResult(string.Format("External Login Provider {0} is not supported.", item.ServiceIdentifier)));
                }
            }            

            return validationResults;
        }
    }
}