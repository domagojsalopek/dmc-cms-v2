using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Web.Attributes;
using Dmc.Cms.Web.Settings;
using Dmc.Cms.Web.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    [NoCache]
    public class ContactController : ControllerBase
    {
        private readonly IContactQueryService _ContactQueryService;

        public ContactController(IContactQueryService contactQueryService, ApplicationUserManager manager, IAppConfig appConfig) 
            : base(manager, appConfig)
        {
            _ContactQueryService = contactQueryService;
        }

        public ActionResult Index()
        {
            return View(new ContactQueryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ContactQueryViewModel model)
        {
            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return View(model);
            }

            

            ContactQuery contactQuery = CreateFromViewModel(model);

            if (User.Identity.IsAuthenticated)
            {
                User user = await GetLoggedInUserAsync();

                if (user != null)
                {
                    contactQuery.UserId = user.Id;
                }
            }

            ServiceResult saveResult = await _ContactQueryService.InsertAsync(contactQuery);

            if (!saveResult.Success)
            {
                AddMessageToViewData(Dmc.Cms.App.MessageType.Error, "An error occured. Please Try Again.");
                return View(model);
            }

            model.Id = contactQuery.Id; // need this to render ... 
            if (!SendEmailWithQueryToSiteOwner(contactQuery, model))
            {
                AddMessageToViewData(Dmc.Cms.App.MessageType.Error, "An error occured. Please Try Again.");
                return View(model);
            }
                                    
            return View("MessageSentSuccess", model);
        }

        private bool SendEmailWithQueryToSiteOwner(ContactQuery contactQuery, ContactQueryViewModel model)
        {
            try
            { 
                string html = this.RenderViewToString("~/Views/Email/AdminContactQuery.cshtml", model);
                EmailClient client = CreateEmailClient();
                client.Send(AppConfig.SiteSettings.MainContactEmail, "New Contact Query", html);

                return true;
            }
            catch (Exception)
            {
                //AddMessageToViewData(MessageType.Error, ex.Message + " " + ex.StackTrace);
                return false;
            }
        }

        //private bool SendEmailWithQueryToCustomer(ContactQuery contactQuery, ContactQueryViewModel model)
        //{
        //    try
        //    {
        //        string html = this.RenderViewToString("~/Views/Email/ContactQuery.cshtml", model);
        //        EmailClient client = CreateEmailClient();
        //        client.Send(contactQuery.Email, "Your Copy of Contact Query", html);

        //        return true;
        //    }
        //    catch (Exception) //TODO: Log
        //    {
        //        return false;
        //    }
        //}

        private ContactQuery CreateFromViewModel(ContactQueryViewModel model)
        {
            return new ContactQuery
            {
                Email = model.Email,
                IP = Request.UserHostAddress,
                Message = model.Message,
                Name = model.Name,
                Subject = model.Subject,
            };
        }
    }
}