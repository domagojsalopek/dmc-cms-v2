using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AssociateExternalLoginViewModel
    {
        public AssociateExternalLoginViewModel()
        {
            AvailableProviders = new List<string>();
        }

        public bool AllowedToAssociate => AvailableProviders.Count > 0;

        public bool HasCredentials => HasPassword && EmailConfirmed;

        public bool AllowedToDisassociate
        {
            get
            {
                int numOfProviders = AlreadyAssociatedProviders.Count;

                if (numOfProviders <= 0)
                {
                    return false;
                }

                if (HasCredentials)
                {
                    return true;
                }

                // doesn't have credentials
                return numOfProviders > 1;
            }
        }

        public List<string> AvailableProviders
        {
            get;
            set;
        }

        public List<string> AlreadyAssociatedProviders
        {
            get;
            set;
        }

        public string ReturnUrl
        {
            get;
            set;
        }

        public bool HasPassword { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}