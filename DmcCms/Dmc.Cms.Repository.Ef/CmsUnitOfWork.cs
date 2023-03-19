using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Dmc.Cms.Model;

namespace Dmc.Cms.Repository.Ef
{
    public class CmsUnitOfWork : ICmsUnitOfWork // should actually have some kind of factory, as base unit of work does, but, wht ... 
    {
        #region Fields

        private readonly DbContext _Context;
        private readonly UnitOfWork _UnitOfWork;

        #endregion

        #region Repository Private Fields

        private IAdvertisementRepository _AdvertisementRepository;
        private IAppRepository _AppRepository;
        private IRatingRepository _RatingRepository;
        private IPostRepository _PostRepository;
        private ICategoryRepository _CategoryRepository;
        private ITagRepository _TagRepository;
        private IPageRepository _PageRepository;
        private IImageRepository _ImageRepository;
        private IContactRepository _ContactRepository;
        private IEventRepository _EventRepository;
        private IOptionRepository _OptionRepository;
        private ICookieConsentRepository _CookieConsentRepository;
        private ICommentRepository _CommentRepository;

        #endregion

        #region Constructor

        public CmsUnitOfWork(DbContext context)
        {
            _Context = context;
            _UnitOfWork = new UnitOfWork(_Context);
        }

        #endregion

        #region Properties

        public ICommentRepository CommentRepository
        {
            get
            {
                if (_CommentRepository == null)
                {
                    _CommentRepository = new CommentRepository(_UnitOfWork.Repository<Comment>(), _Context);
                }
                return _CommentRepository;
            }
        }

        public ICookieConsentRepository CookieConsentRepository
        {
            get
            {
                if (_CookieConsentRepository == null)
                {
                    _CookieConsentRepository = new CookieConsentRepository(_UnitOfWork.Repository<CookieConsent>());
                }
                return _CookieConsentRepository;
            }
        }

        public IAdvertisementRepository AdvertisementRepository
        {
            get
            {
                if (_AdvertisementRepository == null)
                {
                    _AdvertisementRepository = new AdvertisementRepository(_UnitOfWork.Repository<Advertisement>());
                }
                return _AdvertisementRepository;
            }
        }

        public IOptionRepository OptionRepository
        {
            get
            {
                if (_OptionRepository == null)
                {
                    _OptionRepository = new OptionRepository(_UnitOfWork.Repository<Option>());
                }
                return _OptionRepository;
            }
        }

        public IEventRepository EventRepository
        {
            get
            {
                if (_EventRepository == null)
                {
                    _EventRepository = new EventRepository(_UnitOfWork.Repository<Event>());
                }
                return _EventRepository;
            }
        }

        public IAppRepository AppRepository
        {
            get
            {
                if (_AppRepository == null)
                {
                    _AppRepository = new AppRepository(_UnitOfWork.Repository<App>());
                }
                return _AppRepository;
            }
        }

        public IContactRepository ContactRepository
        {
            get
            {
                if (_ContactRepository == null)
                {
                    _ContactRepository = new ContactRepository(_UnitOfWork.Repository<ContactQuery>());
                }
                return _ContactRepository;
            }
        }

        public IRatingRepository RatingRepository
        {
            get
            {
                if (_RatingRepository == null)
                {
                    _RatingRepository = new RatingRepository(_UnitOfWork.Repository<Rating>());
                }
                return _RatingRepository;
            }
        }

        public IImageRepository ImageRepository
        {
            get
            {
                if (_ImageRepository == null)
                {
                    _ImageRepository = new ImageRepository(_UnitOfWork.Repository<Image>());
                }
                return _ImageRepository;
            }
        }

        public IPostRepository PostRepository
        {
            get
            {
                if (_PostRepository == null)
                {
                    _PostRepository = new PostRepository(_UnitOfWork.Repository<Post>(), _Context.Set<Post>());
                }
                return _PostRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_CategoryRepository == null)
                {
                    _CategoryRepository = new CategoryRepository(_UnitOfWork.Repository<Category>(), _Context.Set<Category>());
                }
                return _CategoryRepository;
            }
        }

        public IPageRepository PageRepository
        {
            get
            {
                if (_PageRepository == null)
                {
                    _PageRepository = new PageRepository(_UnitOfWork.Repository<Page>());
                }
                return _PageRepository;
            }
        }

        public ITagRepository TagRepository
        {
            get
            {
                if (_TagRepository == null)
                {
                    _TagRepository = new TagRepository(_UnitOfWork.Repository<Tag>());
                }
                return _TagRepository;
            }
        }

        #endregion

        #region Saving Changes

        public async Task SaveAsync()
        {
            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (Exception) // TODO: Wrap to custom ... 
            {
                throw;
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_UnitOfWork != null)
                    {
                        _UnitOfWork.Dispose();
                    }

                    if (_Context != null)
                    {
                        _Context.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CmsUnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
