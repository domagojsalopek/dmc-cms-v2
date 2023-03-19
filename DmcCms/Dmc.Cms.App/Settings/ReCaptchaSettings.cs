using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class ReCaptchaSettings : IReCaptchaSettings
    {
        public bool UseRecaptcha { get; set; }

        public string RecaptchaSiteKey { get; set; }

        public string RecaptchaSecretKey { get; set; }
    }
}
