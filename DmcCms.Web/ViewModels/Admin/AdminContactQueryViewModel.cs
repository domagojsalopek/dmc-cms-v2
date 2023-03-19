using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class AdminContactQueryViewModel : ICrudViewModel
    {
        public int? Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }        
    }
}