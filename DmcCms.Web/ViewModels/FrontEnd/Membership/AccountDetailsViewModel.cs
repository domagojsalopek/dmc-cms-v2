using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AccountDetailsViewModel
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

        public bool CommentsNameExists
        {
            get
            {
                return !string.IsNullOrWhiteSpace(NickName) || (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName));
            }
        }

        public string CommentsDisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NickName))
                {
                    return NickName;
                }

                if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                }

                return "Anonymous";
            }
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

        public List<string> ExternalLogins
        {
            get;
            set;
        }

        public DateTimeOffset MemberSince
        {
            get;
            set;
        }

        public bool HasExternalLogins
        {
            get;
            set;
        }

        public bool HasPassword
        {
            get;
            set;
        }

        public bool EmailConfirmed
        {
            get;
            set;
        }

        public List<PostViewModel> FavouritePosts
        {
            get;
            set;
        }

        public List<PostViewModel> RatedPosts
        {
            get;
            set;
        }

        public List<UserCommentViewModel> Comments
        {
            get;
            set;
        }
    }
}