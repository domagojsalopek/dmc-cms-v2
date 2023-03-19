using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminExternalLoginSettingViewModel
    {
        [Required]
        public string ServiceIdentifier { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}