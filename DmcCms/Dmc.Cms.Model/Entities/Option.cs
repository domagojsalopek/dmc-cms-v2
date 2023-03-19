using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Option
    {
        #region Constructors

        public Option()
        {
            Modified = DateTimeOffset.UtcNow;
        }

        #endregion

        #region Properties

        public string Name
        {
            get;
            set;
        }

        public string Value
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
