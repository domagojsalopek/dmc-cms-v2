using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class ContactQueryViewModel
    {
        public int Id
        {
            get;
            set;
        }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name
        {
            get;
            set;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get;
            set;
        }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Subject")]
        public string Subject
        {
            get;
            set;
        }

        [Required]
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [Display(Name = "Message")]
        public string Message
        {
            get;
            set;
        }
    }
}