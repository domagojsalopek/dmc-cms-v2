using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using Dmc.Cms.Web.Mappers;
using System.Threading.Tasks;
using Dmc.Cms.App;

namespace Dmc.Cms.Web.Controllers
{
    public class PageController : FrontEndControllerBase<Page, PageViewModel>
    {
        private IPageService _PageService;

        public PageController(IPageService service, ApplicationUserManager manager) : base(service, manager)
        {
            _PageService = service;
        }

        public ActionResult Index()
        {
            return NotFound();
        }

        public async Task<ActionResult> Details(string slug)
        {
            return await EntityDetailsAsync(slug);
        }

        internal override void TransferFromEntityToBrowseViewModel(Page entity, PageViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        internal override void TransferFromEntityToDetailsViewModel(Page entity, PageViewModel viewModel)
        {
            PageMapper.TransferToViewModel(entity, viewModel);
        }
    }
}