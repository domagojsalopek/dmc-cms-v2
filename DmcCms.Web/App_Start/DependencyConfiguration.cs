using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dmc.Core.DI;
using Dmc.Cms.Web.Areas.Admin.Controllers;
using Dmc.Cms.Repository.Ef;
using Dmc.Cms.Repository;
using Dmc.Cms.App.Services;
using System.Data.Entity;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Repository;
using Dmc.Identity;
using Dmc.Identity.Ef;
using Dmc.Cms.App;

namespace Dmc.Cms.Web
{
    public class DependencyConfiguration
    {


        internal static void Configure()
        {

        }

        internal static void Configure(DependencyInjectionContainer container)
        {
            container.Register<CmsContext, CmsContext>();
            container.Register<DbContext, CmsContext>();

            // unit of work ... 
            container.Register<ICmsUnitOfWork, CmsUnitOfWork>();
            container.Register<ICmsUnitOfWorkFactory, CmsUnitOfWorkFactory>();

            // services
            container.Register<ICommentService, CommentService>();
            container.Register<ICategoryService, CategoryService>();
            container.Register<IPageService, PageService>();
            container.Register<ITagService, TagService>();
            container.Register<IPostService, PostService>();
            container.Register<IImageService, ImageService>();
            container.Register<ISearchService, SearchService>();
            container.Register<IContactQueryService, ContactService>();
            container.Register<IAppService, AppService>();
            container.Register<IEventService, EventService>();
            container.Register<IConfigService, ConfigService>();
            container.Register<IAdvertisementService, AdvertisementService>();
            container.Register<ISitemapService, SitemapService>();

            // identity - overcomplicated
            container.Register<ApplicationUserManager, ApplicationUserManager>();
            container.Register<ApplicationUserStore, ApplicationUserStore>();

            container.Register<IUserStore<User>, ApplicationUserStore>();
            container.Register<IIdentityUnitOfWork<User>, IdentityUnitOfWork<User>>();
        }
    }
}