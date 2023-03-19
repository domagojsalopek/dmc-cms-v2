using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Newsletter : EntityBase 
    {
        #region Constructors

        public Newsletter()
        {

        }

        #endregion

        #region Foreign Keys

        public int? UserId
        {
            get;
            set;
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

        public string Content
        {
            get;
            set;
        }

        public NewsletterStatus Status
        {
            get;
            set;
        }

        public DateTimeOffset? LastSent
        {
            get;
            set;
        } 

        public User CreatedBy
        {
            get;
            set;
        }

        #endregion
    }
}
