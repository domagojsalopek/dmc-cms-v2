using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class App : EntityBase
    {
        #region Constructor

        public App()
        {
        }

        #endregion

        #region Properties

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

        public OauthGrantType GrantType
        {
            get;
            set;
        }

        #endregion
    }
}
