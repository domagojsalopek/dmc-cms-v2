using Dmc.Cms.App.Identity;

namespace Dmc.Cms.Web
{
    public interface IApplicationUserManagerFactory
    {
        ApplicationUserManager Create();
    }
}