using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface ICmsUnitOfWork : IDisposable
    {
        ICommentRepository CommentRepository
        {
            get;
        }

        ICookieConsentRepository CookieConsentRepository
        {
            get;
        }

        IAdvertisementRepository AdvertisementRepository
        {
            get;
        }

        IOptionRepository OptionRepository
        {
            get;
        }

        IContactRepository ContactRepository
        {
            get;
        }

        IPostRepository PostRepository
        {
            get;
        }

        ICategoryRepository CategoryRepository
        {
            get;
        }

        IPageRepository PageRepository
        {
            get;
        }

        ITagRepository TagRepository
        {
            get;
        }

        IImageRepository ImageRepository
        {
            get;
        }

        IRatingRepository RatingRepository
        {
            get;
        }

        IAppRepository AppRepository
        {
            get;
        }

        IEventRepository EventRepository
        {
            get;
        }

        Task SaveAsync();
    }
}
