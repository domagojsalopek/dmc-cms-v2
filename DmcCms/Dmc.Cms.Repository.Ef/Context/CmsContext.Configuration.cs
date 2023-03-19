using Dmc.Cms.Repository.Ef.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    public partial class CmsContext
    {
        #region Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // consent
            modelBuilder.Configurations.Add(new CookieConsentConfiguration());

            // ads
            modelBuilder.Configurations.Add(new AdvertisementConfiguration());

            // links
            modelBuilder.Configurations.Add(new LinkConfiguration());

            // Groups and content
            modelBuilder.Configurations.Add(new GeneralContentGroupConfiguration());
            modelBuilder.Configurations.Add(new GeneralContentConfiguration());

            // Users
            modelBuilder.Configurations.Add(new UserConfiguration());

            // Roles
            modelBuilder.Configurations.Add(new UserRoleConfiguration());

            // Logins
            modelBuilder.Configurations.Add(new UserLoginConfiguration());

            // Claims
            modelBuilder.Configurations.Add(new UserClaimConfiguration());

            // Pages
            modelBuilder.Configurations.Add(new PageConfiguration());

            // Posts
            modelBuilder.Configurations.Add(new PostConfiguration());

            // Comments
            modelBuilder.Configurations.Add(new CommentConfiguration());

            // contact query
            modelBuilder.Configurations.Add(new ContactQueryConfiguration());

            // Tags
            modelBuilder.Configurations.Add(new TagConfiguration());

            // Categories
            modelBuilder.Configurations.Add(new CategoryConfiguration());

            // Ratings
            modelBuilder.Configurations.Add(new RatingConfiguration());

            // Options
            modelBuilder.Configurations.Add(new OptionConfiguration());

            // Images
            modelBuilder.Configurations.Add(new ImageConfiguration());

            // newsletter
            modelBuilder.Configurations.Add(new NewsletterConfiguration());
            modelBuilder.Configurations.Add(new NewsletterSubscriptionConfiguration());

            // apps
            modelBuilder.Configurations.Add(new AppConfiguration());

            // events
            modelBuilder.Configurations.Add(new EventConfiguration());
        }

        #endregion
    }
}
