using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class ChangeNicknameViewModel
    {
        public string CurrentNickname
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [RegularExpression(@"^[\w-]+( \w+)*$", ErrorMessage = "Sorry. Only spaces, letters, numbers, dashes and underscores are allowed.")]
        public string Nickname
        {
            get;
            set;
        }
    }
}