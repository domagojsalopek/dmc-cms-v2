using Dmc.Cms.Repository;

namespace Dmc.Cms.Web
{
    public interface ICmsUnitOfWorkFactory
    {
        ICmsUnitOfWork Create();
    }
}