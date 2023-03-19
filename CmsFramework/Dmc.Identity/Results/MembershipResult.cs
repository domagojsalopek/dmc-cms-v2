using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class MembershipResult
    {
        #region Fields

        private static readonly MembershipResult _Success = new MembershipResult { Succeeded = true };
        private List<string> _Errors = new List<string>();

        #endregion

        #region Constructors

        protected MembershipResult() 
        {

        }

        #endregion

        #region Properties

        public bool Succeeded
        {
            get;
            protected set;
        }

        public string Code
        {
            get;
            protected set;
        }

        public List<string> Errors => _Errors;

        #endregion

        #region Static

        public static MembershipResult Success => _Success;

        public static MembershipResult Failed(string code, params string[] errors)
        {
            var result = new MembershipResult { Succeeded = false, Code = code };
            if (errors != null)
            {
                result._Errors.AddRange(errors);
            }
            return result;
        }

        #endregion
    }
}
