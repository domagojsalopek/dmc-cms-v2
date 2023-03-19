using Dmc.Cms.Core;
using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public abstract class EntityBase : IEntity, IModifiedInfo
    {
        #region Constructors

        public EntityBase()
        {
            Created = DateTimeOffset.Now;
            Modified = DateTimeOffset.Now;
        }

        #endregion

        #region Properties

        public int Id
        {
            get;
            set;
        }

        public DateTimeOffset Created
        {
            get;
            set;
        }

        public DateTimeOffset Modified
        {
            get;
            set;
        }

        #endregion
    }
}
