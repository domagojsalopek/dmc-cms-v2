using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class NewsletterSubscriptionConfiguration : DatabaseEntityConfiguration<NewsletterSubscription>
    {
        #region Constructors

        internal NewsletterSubscriptionConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {
            Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(255);
        }

        private void ConfigureRelationships()
        {
        }

        #endregion
    }
}
