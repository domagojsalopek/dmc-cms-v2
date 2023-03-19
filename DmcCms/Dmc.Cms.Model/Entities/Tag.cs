using Dmc.Cms.Core;
using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Tag : ContentBase
    {
        #region Constructors

        public Tag()
        {
            Posts = new List<Post>();
        }

        #endregion

        #region Properties

        public string Title
        {
            get;
            set;
        }

        public ICollection<Post> Posts
        {
            get;
            private set;
        }

        #endregion

        #region Abstract Implementation

        public override bool CanBeDisplayed => Status == ContentStatus.Published;

        #endregion
    }
}
