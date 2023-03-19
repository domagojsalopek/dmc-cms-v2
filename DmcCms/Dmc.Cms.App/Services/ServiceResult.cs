using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class ServiceResult
    {
        private bool _Succeeded = false;
        private List<string> _Errors = new List<string>();

        #region Constructors

        public ServiceResult(params string[] errors)
        {
            _Errors.AddRange(errors);
        }

        #endregion

        #region Properties

        public bool Success => _Succeeded;

        public IEnumerable<string> Errors => _Errors;

        public static ServiceResult Succeeded
        {
            get
            {
                return new ServiceResult { _Succeeded = true };
            }
        }

        #endregion
    }
}
