using Dmc.Cms.Model;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class UserLoginConfiguration : DatabaseEntityConfiguration<IdentityLogin>
    {
        #region Private Fields

        private const string LoginProviderAndProviderIdIndexName = "IX_ProviderKey";

        #endregion

        #region Constructors

        internal UserLoginConfiguration()
        {
            ConfigureProperties();
            ConfigureRelationships();
        }

        #endregion

        #region Private Methods

        private void ConfigureProperties()
        {            
            Property(o => o.LoginProvider)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(LoginProviderAndProviderIdIndexName, 1) { IsUnique = true }));

            Property(o => o.ProviderUniqueId)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute(LoginProviderAndProviderIdIndexName, 2) { IsUnique = true }));

            Property(o => o.ScreenName)
                .IsOptional()
                .HasMaxLength(255);
        }

        private void ConfigureRelationships()
        {
            // relationship with user configured in userconfiguration. 
        }

        #endregion
    }
}
