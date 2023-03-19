using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class EmailSettings : IEmailSettings
    {
        #region Properties

        public string SmtpHost
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string SendFromEmail
        {
            get;
            set;
        }

        public string SendFromName
        {
            get;
            set;
        }

        public int? SmtpPort
        {
            get;
            set;
        }

        #endregion 
    }
}
