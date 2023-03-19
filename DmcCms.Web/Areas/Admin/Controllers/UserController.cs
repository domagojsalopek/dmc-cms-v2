using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class UserController : ControllerBase
    {
        #region Fields

        private readonly ApplicationUserManager _UserManager;
        private readonly IIdentityUnitOfWork<User> _UnitOfWork;

        #endregion

        #region Constructors

        public UserController(ApplicationUserManager userManager, IIdentityUnitOfWork<User> unitOfWork) 
            : base(userManager)
        {
            _UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #endregion

        #region Web Methods

        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}