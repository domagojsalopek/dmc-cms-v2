namespace Dmc.Cms.Repository.Ef.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CookieConsent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueId = c.Guid(nullable: false),
                        EncryptedValue = c.String(nullable: false, maxLength: 4000),
                        IpAddress = c.String(maxLength: 255),
                        UserAgent = c.String(maxLength: 1000),
                        RequestUrl = c.String(maxLength: 2000),
                        ConsentGiven = c.Boolean(nullable: false),
                        ConsentGivenDateUtc = c.DateTimeOffset(nullable: false, precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.Advertisement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        AdvertisementType = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Html = c.String(nullable: false, storeType: "ntext"),
                        IsVisible = c.Boolean(nullable: false),
                        VisibleFrom = c.DateTimeOffset(precision: 7),
                        VisibleTo = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.Link",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LinkType = c.Int(nullable: false),
                        Location = c.String(nullable: false, maxLength: 2000),
                        Text = c.String(nullable: false, maxLength: 255),
                        Tooltip = c.String(maxLength: 255),
                        CssClass = c.String(maxLength: 255),
                        Order = c.Int(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GeneralContentGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        IntroImageId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Image", t => t.IntroImageId)
                .ForeignKey("dbo.GeneralContentGroup", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.IntroImageId)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.GeneralContent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContentGroupId = c.Int(nullable: false),
                        PreviewImageId = c.Int(),
                        DetailImageId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Content = c.String(nullable: false, storeType: "ntext"),
                        Order = c.Int(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeneralContentGroup", t => t.ContentGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Image", t => t.DetailImageId)
                .ForeignKey("dbo.Image", t => t.PreviewImageId)
                .Index(t => t.ContentGroupId)
                .Index(t => t.PreviewImageId)
                .Index(t => t.DetailImageId)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        AltText = c.String(maxLength: 4000),
                        Caption = c.String(maxLength: 4000),
                        SmallImage = c.String(),
                        LargeImage = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 255),
                        LastName = c.String(maxLength: 255),
                        NickName = c.String(maxLength: 255),
                        About = c.String(maxLength: 2000),
                        ImageUrl = c.String(maxLength: 2000),
                        UniqueId = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        PasswordHash = c.Binary(maxLength: 255),
                        SecurityStamp = c.String(nullable: false, maxLength: 255),
                        EmailConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEnabled = c.Boolean(nullable: false),
                        LoginFailedCount = c.Int(nullable: false),
                        LockoutEnd = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UniqueId, unique: true)
                .Index(t => t.Email, unique: true)
                .Index(t => t.UserName, unique: true);
            
            CreateTable(
                "dbo.IdentityClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(nullable: false, maxLength: 255),
                        ClaimValue = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        UserId = c.Int(),
                        Status = c.Int(nullable: false),
                        Author = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        Text = c.String(nullable: false, maxLength: 4000),
                        IP = c.String(maxLength: 64),
                        Approved = c.Boolean(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comment", t => t.ParentId)
                .ForeignKey("dbo.Post", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityUser", t => t.UserId)
                .Index(t => t.PostId)
                .Index(t => t.ParentId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Post",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(),
                        PreviewImageId = c.Int(),
                        DetailImageId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Content = c.String(nullable: false, storeType: "ntext"),
                        IsCommentingEnabled = c.Boolean(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.AuthorId)
                .ForeignKey("dbo.Image", t => t.DetailImageId)
                .ForeignKey("dbo.Image", t => t.PreviewImageId)
                .Index(t => t.AuthorId)
                .Index(t => t.PreviewImageId)
                .Index(t => t.DetailImageId)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        IntroImageId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Image", t => t.IntroImageId)
                .ForeignKey("dbo.Category", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.IntroImageId)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.Rating",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                        IsLike = c.Boolean(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Post", t => t.PostId)
                .ForeignKey("dbo.IdentityUser", t => t.UserId)
                .Index(t => new { t.UserId, t.PostId }, unique: true, name: "IX_UserIdPostId");
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.IdentityLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        LoginProvider = c.String(nullable: false, maxLength: 255),
                        ProviderUniqueId = c.String(nullable: false, maxLength: 255),
                        ScreenName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => new { t.LoginProvider, t.ProviderUniqueId }, unique: true, name: "IX_ProviderKey");
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(),
                        PreviewImageId = c.Int(),
                        DetailImageId = c.Int(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Content = c.String(nullable: false, storeType: "ntext"),
                        Order = c.Int(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.AuthorId)
                .ForeignKey("dbo.Image", t => t.DetailImageId)
                .ForeignKey("dbo.Image", t => t.PreviewImageId)
                .Index(t => t.AuthorId)
                .Index(t => t.PreviewImageId)
                .Index(t => t.DetailImageId)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.ContactQuery",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 255),
                        Email = c.String(nullable: false, maxLength: 255),
                        Subject = c.String(nullable: false, maxLength: 255),
                        Message = c.String(nullable: false, maxLength: 4000),
                        IP = c.String(nullable: false, maxLength: 64),
                        AttachmentPath = c.String(maxLength: 4000),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Option",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 255),
                        Value = c.String(nullable: false, maxLength: 4000),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Newsletter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Content = c.String(storeType: "ntext"),
                        Status = c.Int(nullable: false),
                        LastSent = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUser", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.NewsletterSubscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.App",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        ClientId = c.String(nullable: false, maxLength: 255),
                        ClientSecret = c.String(nullable: false, maxLength: 1000),
                        GrantType = c.Int(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientId, unique: true);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageId = c.Int(),
                        EventType = c.Int(nullable: false),
                        EventDate = c.DateTime(nullable: false),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 4000),
                        Content = c.String(nullable: false, storeType: "ntext"),
                        Order = c.Int(nullable: false),
                        Slug = c.String(nullable: false, maxLength: 255),
                        UniqueId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Published = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        Modified = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Image", t => t.ImageId)
                .Index(t => t.ImageId)
                .Index(t => t.EventDate)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.UniqueId, unique: true);
            
            CreateTable(
                "dbo.ContentImages",
                c => new
                    {
                        ContentId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ContentId, t.ImageId })
                .ForeignKey("dbo.GeneralContent", t => t.ContentId, cascadeDelete: true)
                .ForeignKey("dbo.Image", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ContentId)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.PostCategories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CategoryId, t.PostId })
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Post", t => t.PostId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.PostImages",
                c => new
                    {
                        PostId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PostId, t.ImageId })
                .ForeignKey("dbo.Post", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.Image", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.PostTags",
                c => new
                    {
                        PostId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PostId, t.TagId })
                .ForeignKey("dbo.Post", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.UserFavouritePosts",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.PostId })
                .ForeignKey("dbo.IdentityUser", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Post", t => t.PostId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.IdentityUsersInRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.IdentityUser", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityRole", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.PageImages",
                c => new
                    {
                        PageId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PageId, t.ImageId })
                .ForeignKey("dbo.Page", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.Image", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.PageId)
                .Index(t => t.ImageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Event", "ImageId", "dbo.Image");
            DropForeignKey("dbo.Newsletter", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.ContactQuery", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.Page", "PreviewImageId", "dbo.Image");
            DropForeignKey("dbo.PageImages", "ImageId", "dbo.Image");
            DropForeignKey("dbo.PageImages", "PageId", "dbo.Page");
            DropForeignKey("dbo.Page", "DetailImageId", "dbo.Image");
            DropForeignKey("dbo.Page", "AuthorId", "dbo.IdentityUser");
            DropForeignKey("dbo.IdentityUsersInRoles", "RoleId", "dbo.IdentityRole");
            DropForeignKey("dbo.IdentityUsersInRoles", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.IdentityLogin", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.UserFavouritePosts", "PostId", "dbo.Post");
            DropForeignKey("dbo.UserFavouritePosts", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.Comment", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.PostTags", "TagId", "dbo.Tag");
            DropForeignKey("dbo.PostTags", "PostId", "dbo.Post");
            DropForeignKey("dbo.Rating", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.Rating", "PostId", "dbo.Post");
            DropForeignKey("dbo.Post", "PreviewImageId", "dbo.Image");
            DropForeignKey("dbo.PostImages", "ImageId", "dbo.Image");
            DropForeignKey("dbo.PostImages", "PostId", "dbo.Post");
            DropForeignKey("dbo.Post", "DetailImageId", "dbo.Image");
            DropForeignKey("dbo.Comment", "PostId", "dbo.Post");
            DropForeignKey("dbo.PostCategories", "PostId", "dbo.Post");
            DropForeignKey("dbo.PostCategories", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Category", "ParentId", "dbo.Category");
            DropForeignKey("dbo.Category", "IntroImageId", "dbo.Image");
            DropForeignKey("dbo.Post", "AuthorId", "dbo.IdentityUser");
            DropForeignKey("dbo.Comment", "ParentId", "dbo.Comment");
            DropForeignKey("dbo.IdentityClaim", "UserId", "dbo.IdentityUser");
            DropForeignKey("dbo.GeneralContentGroup", "ParentId", "dbo.GeneralContentGroup");
            DropForeignKey("dbo.GeneralContentGroup", "IntroImageId", "dbo.Image");
            DropForeignKey("dbo.GeneralContent", "PreviewImageId", "dbo.Image");
            DropForeignKey("dbo.ContentImages", "ImageId", "dbo.Image");
            DropForeignKey("dbo.ContentImages", "ContentId", "dbo.GeneralContent");
            DropForeignKey("dbo.GeneralContent", "DetailImageId", "dbo.Image");
            DropForeignKey("dbo.GeneralContent", "ContentGroupId", "dbo.GeneralContentGroup");
            DropIndex("dbo.PageImages", new[] { "ImageId" });
            DropIndex("dbo.PageImages", new[] { "PageId" });
            DropIndex("dbo.IdentityUsersInRoles", new[] { "RoleId" });
            DropIndex("dbo.IdentityUsersInRoles", new[] { "UserId" });
            DropIndex("dbo.UserFavouritePosts", new[] { "PostId" });
            DropIndex("dbo.UserFavouritePosts", new[] { "UserId" });
            DropIndex("dbo.PostTags", new[] { "TagId" });
            DropIndex("dbo.PostTags", new[] { "PostId" });
            DropIndex("dbo.PostImages", new[] { "ImageId" });
            DropIndex("dbo.PostImages", new[] { "PostId" });
            DropIndex("dbo.PostCategories", new[] { "PostId" });
            DropIndex("dbo.PostCategories", new[] { "CategoryId" });
            DropIndex("dbo.ContentImages", new[] { "ImageId" });
            DropIndex("dbo.ContentImages", new[] { "ContentId" });
            DropIndex("dbo.Event", new[] { "UniqueId" });
            DropIndex("dbo.Event", new[] { "Slug" });
            DropIndex("dbo.Event", new[] { "EventDate" });
            DropIndex("dbo.Event", new[] { "ImageId" });
            DropIndex("dbo.App", new[] { "ClientId" });
            DropIndex("dbo.Newsletter", new[] { "UserId" });
            DropIndex("dbo.ContactQuery", new[] { "UserId" });
            DropIndex("dbo.Page", new[] { "UniqueId" });
            DropIndex("dbo.Page", new[] { "Slug" });
            DropIndex("dbo.Page", new[] { "DetailImageId" });
            DropIndex("dbo.Page", new[] { "PreviewImageId" });
            DropIndex("dbo.Page", new[] { "AuthorId" });
            DropIndex("dbo.IdentityLogin", "IX_ProviderKey");
            DropIndex("dbo.IdentityLogin", new[] { "UserId" });
            DropIndex("dbo.Tag", new[] { "UniqueId" });
            DropIndex("dbo.Tag", new[] { "Slug" });
            DropIndex("dbo.Rating", "IX_UserIdPostId");
            DropIndex("dbo.Category", new[] { "UniqueId" });
            DropIndex("dbo.Category", new[] { "Slug" });
            DropIndex("dbo.Category", new[] { "IntroImageId" });
            DropIndex("dbo.Category", new[] { "ParentId" });
            DropIndex("dbo.Post", new[] { "UniqueId" });
            DropIndex("dbo.Post", new[] { "Slug" });
            DropIndex("dbo.Post", new[] { "DetailImageId" });
            DropIndex("dbo.Post", new[] { "PreviewImageId" });
            DropIndex("dbo.Post", new[] { "AuthorId" });
            DropIndex("dbo.Comment", new[] { "UserId" });
            DropIndex("dbo.Comment", new[] { "ParentId" });
            DropIndex("dbo.Comment", new[] { "PostId" });
            DropIndex("dbo.IdentityClaim", new[] { "UserId" });
            DropIndex("dbo.IdentityUser", new[] { "UserName" });
            DropIndex("dbo.IdentityUser", new[] { "Email" });
            DropIndex("dbo.IdentityUser", new[] { "UniqueId" });
            DropIndex("dbo.GeneralContent", new[] { "UniqueId" });
            DropIndex("dbo.GeneralContent", new[] { "Slug" });
            DropIndex("dbo.GeneralContent", new[] { "DetailImageId" });
            DropIndex("dbo.GeneralContent", new[] { "PreviewImageId" });
            DropIndex("dbo.GeneralContent", new[] { "ContentGroupId" });
            DropIndex("dbo.GeneralContentGroup", new[] { "UniqueId" });
            DropIndex("dbo.GeneralContentGroup", new[] { "Slug" });
            DropIndex("dbo.GeneralContentGroup", new[] { "IntroImageId" });
            DropIndex("dbo.GeneralContentGroup", new[] { "ParentId" });
            DropIndex("dbo.Advertisement", new[] { "UniqueId" });
            DropIndex("dbo.CookieConsent", new[] { "UniqueId" });
            DropTable("dbo.PageImages");
            DropTable("dbo.IdentityUsersInRoles");
            DropTable("dbo.UserFavouritePosts");
            DropTable("dbo.PostTags");
            DropTable("dbo.PostImages");
            DropTable("dbo.PostCategories");
            DropTable("dbo.ContentImages");
            DropTable("dbo.Event");
            DropTable("dbo.App");
            DropTable("dbo.NewsletterSubscription");
            DropTable("dbo.Newsletter");
            DropTable("dbo.Option");
            DropTable("dbo.ContactQuery");
            DropTable("dbo.Page");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.IdentityLogin");
            DropTable("dbo.Tag");
            DropTable("dbo.Rating");
            DropTable("dbo.Category");
            DropTable("dbo.Post");
            DropTable("dbo.Comment");
            DropTable("dbo.IdentityClaim");
            DropTable("dbo.IdentityUser");
            DropTable("dbo.Image");
            DropTable("dbo.GeneralContent");
            DropTable("dbo.GeneralContentGroup");
            DropTable("dbo.Link");
            DropTable("dbo.Advertisement");
            DropTable("dbo.CookieConsent");
        }
    }
}
