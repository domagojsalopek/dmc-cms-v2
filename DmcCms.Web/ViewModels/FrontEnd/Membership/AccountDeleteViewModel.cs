using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AccountDeleteViewModel
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

        public string Email
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }

                if (!string.IsNullOrWhiteSpace(NickName))
                {
                    return NickName;
                }

                return Email;
            }
        }

        public DateTimeOffset MemberSince
        {
            get;
            set;
        }

        public DateTime TokenExpires
        {
            get;
            set;
        }

        [Required]
        public string Token
        {
            get;
            set;
        }
    }
}