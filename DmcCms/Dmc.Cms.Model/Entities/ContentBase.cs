using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public abstract class ContentBase : EntityBase, IContent
    {
        #region Constructors

        protected ContentBase()
        {
            Status = ContentStatus.Draft;
            UniqueId = Guid.NewGuid();
            Published = DateTimeOffset.Now;
        }

        #endregion

        #region Properties

        public virtual string Slug
        {
            get;
            set;
        }

        public Guid UniqueId
        {
            get;
            private set;
        }

        public ContentStatus Status
        {
            get;
            set;
        }

        public virtual DateTimeOffset? Published
        {
            get;
            set;
        }

        #endregion

        #region Abstract Properties

        public abstract bool CanBeDisplayed { get; }

        #endregion
    }
}
