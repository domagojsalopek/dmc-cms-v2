using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminAppViewModel : CrudViewModelBase
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ClientId
        {
            get;
            set;
        }

        public string ClientSecret
        {
            get;
            set;
        }

        //public OauthGrantType GrantType
        //{
        //    get;
        //    set;
        //}
    }
}