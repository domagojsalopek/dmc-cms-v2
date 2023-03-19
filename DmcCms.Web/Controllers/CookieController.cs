using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class CookieController : Controller
    {
        const string CMSCookieConsentCookieName = CmsUtilities.CMSCookieConsentCookieName;
        const int CookieExpiresInDays = 365;

        // we do this not to ask the database every time
        private readonly ICmsUnitOfWorkFactory _CmsUnitOfWorkFactory;

        // taken from stack overflow
        // https://stackoverflow.com/questions/544450/detecting-honest-web-crawlers
        private static readonly List<string> Crawlers3 = new List<string>()
        {
            "bot","crawler","spider","80legs","baidu","yahoo! slurp","ia_archiver","mediapartners-google",
            "lwp-trivial","nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne",
            "atn_worldwide","atomz","bjaaland","ukonline","calif","combine","cosmos","cusco",
            "cyberspyder","digger","grabber","downloadexpress","ecollector","ebiness","esculapio",
            "esther","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
            "gcreep","golem","griffon","gromit","gulliver","gulper","whowhere","havindex","hotwired",
            "htdig","ingrid","informant","inspectorwww","iron33","teoma","ask jeeves","jeeves",
            "image.kapsi.net","kdd-explorer","label-grabber","larbin","linkidator","linkwalker",
            "lockon","marvin","mattie","mediafox","merzscope","nec-meshexplorer","udmsearch","moget",
            "motor","muncher","muninn","muscatferret","mwdsearch","sharp-info-agent","webmechanic",
            "netscoop","newscan-online","objectssearch","orbsearch","packrat","pageboy","parasite",
            "patric","pegasus","phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus",
            "raven","roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au",
            "searchprocess","senrigan","shagseeker","site valet","skymob","slurp","snooper","speedy",
            "curl_image_client","suke","www.sygol.com","tach_bw","templeton","titin","topiclink","udmsearch",
            "urlck","valkyrie libwww-perl","verticrawl","victoria","webscout","voyager","crawlpaper",
            "webcatcher","t-h-u-n-d-e-r-s-t-o-n-e","webmoose","pagesinventory","webquest","webreaper",
            "webwalker","winona","occam","robi","fdse","jobo","rhcs","gazz","dwcp","yeti","fido","wlm",
            "wolp","wwwc","xget","legs","curl","webs","wget","sift","cmc"
        };
        

        public CookieController(ICmsUnitOfWorkFactory cmsUnitOfWorkFactory)
        {
            _CmsUnitOfWorkFactory = cmsUnitOfWorkFactory;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Enable(string requestUrl = null, string userAgent = null)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            if (!IsCookieSet())
            {
                return await CreateAndSaveCookie(requestUrl, userAgent);
            }

            // here we know cookie exists. validate it.
            if (!IsCookieValid())
            {
                DeleteCookie();
                return Failed();
            }

            return Succeeded();
        }

        private bool IsCookieValid()
        {
            var cookie = Request.Cookies.Get(CMSCookieConsentCookieName);

            if (cookie == null)
            {
                return false; // here we presume that it's checked earlier if exists
            }

            string value = Request.Cookies[CMSCookieConsentCookieName].Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return TryValidateCookieValue(value);
        }

        private bool TryValidateCookieValue(string value)
        {
            try
            {
                string[] values = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (values == null || values.Length != 5)
                {
                    return false;
                }

                if (!Guid.TryParse(values[2], out Guid id))
                {
                    return false;
                }

                if (!bool.TryParse(values[4], out bool consentGiven))
                {
                    return false;
                }

                if (!consentGiven)
                {
                    return false;
                }

                // in the end value is -> SIGNATURE|ENCRYPTED_VALUE|UNIQUE_ID|TIME_STRING|CONSENT_GIVEN
                string signatureFromCookie = values[0];
                string encryptedValue = values[1];
                string timeAsString = values[3];

                string serverSignature = GenerateSignature(id, timeAsString, encryptedValue);

                if (!serverSignature.Equals(signatureFromCookie, StringComparison.Ordinal))
                {
                    return false;
                }

                // do we even need anything else? signature was a mistake.
                return DecryptAndValidate(encryptedValue, id, timeAsString);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool DecryptAndValidate(string encryptedValue, Guid idFromCookie, string timeAsString)
        {
            // ENCRYPTED IS -> GUID|DATE|TRUE
            if (!TryDecrypt(encryptedValue, out string decryptedString))
            {
                return false;
            }

            try
            {
                string[] splitted = decryptedString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitted == null || splitted.Length != 3)
                {
                    return false;
                }

                if (!Guid.TryParse(splitted[0], out Guid idFromEncryptedString))
                {
                    return false;
                }

                if (!bool.TryParse(splitted[2], out bool accpetedFromCookie))
                {
                    return false;
                }

                if (!accpetedFromCookie)
                {
                    return false;
                }

                return idFromCookie.Equals(idFromEncryptedString) && timeAsString.Equals(splitted[1], StringComparison.Ordinal);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool TryDecrypt(string encryptedValue, out string decryptedString)
        {
            decryptedString = null;

            if (string.IsNullOrWhiteSpace(encryptedValue))
            {
                return false;
            }

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedValue);
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.LocalMachine);
                decryptedString = Encoding.UTF8.GetString(decryptedBytes);

                return !string.IsNullOrWhiteSpace(decryptedString);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GenerateSignature(Guid id, string timestamp, string encryptedValue)
        {
            string text = string.Format("{0}{1}{0}"
                , timestamp.ToUpper()
                , id.ToString().ToUpper()
            );

            string stringToHash = string.Format("{0}.{1}"
                , text
                , encryptedValue);

            byte[] bytes = Encoding.UTF8.GetBytes(stringToHash);
            using (SHA256Managed hashstring = new SHA256Managed())
            {
                byte[] hash = hashstring.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private async Task<ActionResult> CreateAndSaveCookie(string requestUrl, string userAgent)
        {
            var consent = new CookieConsent
            {
                ConsentGiven = true,
                IpAddress = MaskIpAddress(Request.UserHostAddress),
                RequestUrl = requestUrl,
                UserAgent = Request.UserAgent,
                UniqueId = Guid.NewGuid(),
                ConsentGivenDateUtc = DateTimeOffset.UtcNow
            };

            if (!await GenerateEncryptedValueAndSaveIfNeeded(consent, userAgent))
            {
                return Failed();
            }

            SetCookie(consent);
            return Succeeded();
        }

        private async Task<bool> GenerateEncryptedValueAndSaveIfNeeded(CookieConsent consent, string userAgent)
        {
            string dateAsString = consent.ConsentGivenDateUtc.ToUniversalTime().UtcTicks.ToString();

            string encryptedValueRaw = string.Format("{0}|{1}|{2}"
                , consent.UniqueId.ToString()
                , dateAsString
                , consent.ConsentGiven.ToString());

            byte[] finalBytes = Encoding.UTF8.GetBytes(encryptedValueRaw);
            byte[] encryptedBytes = ProtectedData.Protect(finalBytes, null, DataProtectionScope.LocalMachine);

            // we don't store the signature. we calculate it.
            consent.EncryptedValue = Convert.ToBase64String(encryptedBytes);

            string ua = userAgent.ToLower();
            bool iscrawler = Crawlers3.Exists(x => ua.Contains(x));

            if (iscrawler) // we don't need to save bot consent
            {
                return true; 
            }

            try
            {
                using (var uow = _CmsUnitOfWorkFactory.Create())
                {
                    uow.CookieConsentRepository.Insert(consent);
                    await uow.SaveAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string MaskIpAddress(string userHostAddress)
        {
            if (string.IsNullOrWhiteSpace(userHostAddress))
            {
                return null;
            }

            if (userHostAddress.Contains("."))
            {
                return string.Format("{0}.XXX", userHostAddress.Substring(0, userHostAddress.LastIndexOf(".")));
            }

            return string.Format("{0}:XXXX", userHostAddress.Substring(0, userHostAddress.LastIndexOf(":")));
        }

        private void SetCookie(CookieConsent cookieConsent)
        {
            string dateAsString = cookieConsent.ConsentGivenDateUtc.ToUniversalTime().UtcTicks.ToString();
            string signature = GenerateSignature(cookieConsent.UniqueId, dateAsString, cookieConsent.EncryptedValue);
            string cookieValue = string.Format("{0}|{1}", signature, cookieConsent.Serialize());

            // in the end value is -> SIGNATURE|ENCRYPTED_VALUE|UNIQUE_ID|TIME_STRING|CONSENT_GIVEN

            Response.Cookies.Add(new HttpCookie(CMSCookieConsentCookieName)
            {
                Expires = DateTime.Now.AddDays(CookieExpiresInDays),
                Value = cookieValue,
                HttpOnly = true,
#if !DEBUG
                Secure = true
#endif
            });
        }

        private void DeleteCookie()
        {
            var cookieToRemove = new HttpCookie(CMSCookieConsentCookieName)
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = false,
                Secure = false,
            };
            Response.Cookies.Set(cookieToRemove);
        }

        private ActionResult Failed()
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new { Success = false }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        private ActionResult Succeeded()
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new { Success = true }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        private bool IsCookieSet()
        {
            return Request.Cookies.Get(CMSCookieConsentCookieName) != null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Disable()
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            //if (Request.Cookies[CookieConsentScriptCookieName] != null)
            //{
            //    var cookie = new HttpCookie(CookieConsentScriptCookieName, "deny")
            //    {
            //        HttpOnly = false,
            //        Secure = false,
            //        Expires = DateTime.Now.AddDays(CookieExpiresInDays)
            //    };
            //    Response.Cookies.Set(cookie);
            //}

            if (Request.Cookies[CMSCookieConsentCookieName] != null)
            {
                var cookieToRemove = new HttpCookie(CMSCookieConsentCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = false,
                    Secure = false,
                };
                Response.Cookies.Set(cookieToRemove);
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new { Success = true }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }
    }
}