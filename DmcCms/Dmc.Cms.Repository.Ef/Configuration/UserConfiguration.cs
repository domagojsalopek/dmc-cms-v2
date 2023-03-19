using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef.Configuration
{
    internal class UserConfiguration : DatabaseEntityConfiguration<User>
    {
        #region Constructors

        internal UserConfiguration()
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
                .HasMaxLength(255)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Email") { IsUnique = true }));

            Property(o => o.UserName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_UserName") { IsUnique = true }));

            Property(o => o.UniqueId)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_UniqueId") { IsUnique = true }));

            Property(o => o.PasswordHash)
                .IsOptional() // to allow external logins without a password
                .HasMaxLength(255);

            Property(o => o.SecurityStamp)
                .IsRequired()
                .HasMaxLength(255);

            Property(o => o.FirstName)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.LastName)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.NickName)
                .IsOptional()
                .HasMaxLength(255);

            Property(o => o.About)
                .IsOptional()
                .HasMaxLength(2000);

            Property(o => o.ImageUrl)
                .IsOptional()
                .HasMaxLength(2000);
        }

        private void ConfigureRelationships() 
        {
            HasMany(o => o.FavouritePosts)
                .WithMany().Map(
                    at => at.MapLeftKey("UserId")
                        .MapRightKey("PostId")
                        .ToTable("UserFavouritePosts"));

            // this all should be configured with a helper!!

            // Many to many users
            HasMany(a => a.Roles).WithMany().Map(
                at => at.MapLeftKey("UserId")
                        .MapRightKey("RoleId")
                        .ToTable("IdentityUsersInRoles"));

            // Has many logins
            HasMany(o => o.Logins)
                .WithRequired()
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(true);

            // Has many claims
            HasMany(o => o.Claims)
                .WithRequired()
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(true);

            // For consistency we specify table name
            ToTable("IdentityUser");
        }

        #endregion
    }
}
