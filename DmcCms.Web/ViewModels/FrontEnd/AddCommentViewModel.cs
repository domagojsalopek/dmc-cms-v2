using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AddCommentViewModel
    {
        [Required]
        public int? PostId
        {
            get;
            set;
        }

        public int? ReplyTo
        {
            get;
            set;
        }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Comment
        {
            get;
            set;
        }
    }
}