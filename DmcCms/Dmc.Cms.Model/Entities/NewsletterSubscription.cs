using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class NewsletterSubscription : IEntity
    {
        public int Id
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public DateTimeOffset Created
        {
            get;
            set;
        }
    }
}
