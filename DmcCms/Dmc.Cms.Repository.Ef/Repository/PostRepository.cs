using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Dmc.Repository;
using Dmc.Repository.Ef;
using System.Data.Entity.SqlServer;

namespace Dmc.Cms.Repository.Ef
{
    internal class PostRepository : ContentRepositoryBase<Post>, IPostRepository //TODO: maybe we can move this even to the cms.repository? but if we do then, we'll also need count methods etc on repository ... 
    {
        private readonly IDbSet<Post> _DbSet;
        private static readonly Random _Random = new Random();

        #region Constructors

        internal PostRepository(IRepository<Post> repository, IDbSet<Post> dbSet) 
            : base(repository)
        {
            _DbSet = dbSet;
        }

        #endregion

        public async Task<IEnumerable<CommentCountInfo>> CountCommentsForPostIdsAsync(int[] postIds)
        {
            return await _DbSet
                .Where(o => postIds.Contains(o.Id))
                .Select(s => new CommentCountInfo
                {
                    NumberOfComments = s.Comments.Count(c => c.Approved),
                    OwnerId = s.Id
                })
                .ToListAsync();
        }

        public override async Task<IEnumerable<Post>> GetPagedAsync(int page, int perPage)
        {
            return await Repository
                .Query()
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                //.Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)

                //.Include(o => o.DetailImage)
                .OrderBy(o => o.OrderByDescending(d => d.Published))
                .GetPagedEntitiesAsync(page, perPage);
        }

        public async Task<IEnumerable<Post>> GetLatestbyPostIdsAsync(int[] postIds, int howManyLatest)
        {
            return await _DbSet
                .Where(o => 
                    postIds.Contains(o.Id)
                )
                .Include(o => o.Author)

                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)

                .OrderByDescending(o => o.Published)
                .Take(howManyLatest)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllByPostIdsAsync(int[] postIds)
        {
            return await _DbSet
                .Where(o =>
                    postIds.Contains(o.Id)
                )

                .Include(o => o.PreviewImage)
                .OrderByDescending(o => o.Published)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllByPostIdsWithDetailsAsync(int[] postIds)
        {
            return await _DbSet
                .Where(o =>
                    postIds.Contains(o.Id)
                )
                .Include(o => o.PreviewImage)
                .Include(o => o.Author)

                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .OrderByDescending(o => o.Published)
                .ToListAsync();
        }

        //TODO: we have to see what to eager load. Comments for example are stupid to eager load on lists

        public override async Task<Post> GetByIdAsync(int id)
        {
            return await Repository.Query()
                .Filter(o => o.Id == id)
                .Include(o => o.Author)
                .Include(o => o.Ratings)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)
                .Include(o => o.DetailImage)
                .FirstOrDefaultAsync();
        }

        public override async Task<Post> GetBySlugAsync(string slug)
        {
            return await Repository.Query()
                .Filter(o => o.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                .Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)

                .Include(o => o.DetailImage)
                .FirstOrDefaultAsync();
        }

        public async Task<Post> GetNextPostAsync(int currentPostId)
        {
            return await Repository.Query()
                .Filter(o => 
                    o.Published > _DbSet.Where(s => s.Id == currentPostId).Select(s => s.Published).FirstOrDefault()
                    &&
                    o.Status == Core.ContentStatus.Published
                )
                .OrderBy(o => 
                    o.OrderBy(ord => ord.Published)
                )
                .FirstOrDefaultAsync();
        }

        public async Task<Post> GetPreviousPostAsync(int currentPostId)
        {
            return await Repository.Query()
                .Filter(o => 
                    o.Published < _DbSet.Where(s => s.Id == currentPostId).Select(s => s.Published).FirstOrDefault()
                    &&
                    o.Status == Core.ContentStatus.Published
                )
                .OrderBy(o =>
                    o.OrderByDescending(ord => ord.Published)
                )
                .FirstOrDefaultAsync();
        }

        public override Task<IEnumerable<Post>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            var query = GetDraftsAndPublishedQuery()
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                //.Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage);

            return query.GetPagedEntitiesAsync(page, perPage);
        }

        public override Task<IEnumerable<Post>> GetPagedPublishedAsync(int page, int perPage)
        {
            var query = GetPublishedQuery()
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                //.Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage);

            return query.GetPagedEntitiesAsync(page, perPage);
        }

        public Task<int> CountInCategory(int categoryId, bool includeDrafts)
        {
            IQuery<Post> startQuery = includeDrafts
                ? GetDraftsAndPublishedQuery()
                : GetPublishedQuery();

            return startQuery
                .Filter(o => o.Categories.Any(s => s.Id == categoryId))
                .CountAsync();
        }

        public Task<IEnumerable<Post>> GetPagedInCategoryAsync(int categoryId, int page, int perPage, bool includeDrafts)
        {
            IQuery<Post> startQuery = includeDrafts 
                ? GetDraftsAndPublishedQuery() 
                : GetPublishedQuery();

            return startQuery
                .Filter(o => o.Categories.Any(s => s.Id == categoryId))
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                //.Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)
                .GetPagedEntitiesAsync(page, perPage);
        }

        public Task<int> CountPostsWithTagAsync(int tagId, bool includeDrafts)
        {
            IQuery<Post> startQuery = includeDrafts
                ? GetDraftsAndPublishedQuery()
                : GetPublishedQuery();

            return startQuery
                .Filter(o => o.Tags.Any(s => s.Id == tagId))
                .CountAsync();
        }

        public Task<IEnumerable<Post>> GetPostsWithTagAsync(int tagId, int page, int perPage, bool includeDrafts)
        {
            IQuery<Post> startQuery = includeDrafts
                ? GetDraftsAndPublishedQuery()
                : GetPublishedQuery();

            return startQuery
                .Filter(o => o.Tags.Any(s => s.Id == tagId))
                .Include(o => o.Author)
                //.Include(o => o.Ratings)
                //.Include(o => o.Comments)
                .Include(o => o.Categories)
                .Include(o => o.Tags)
                .Include(o => o.PreviewImage)
                .GetPagedEntitiesAsync(page, perPage);
        }

        public async Task<IEnumerable<Post>> SearchPostsAsync(string[] search, int page, int perPage)
        {
            const decimal titleWeight = 1.2m, descriptionWeight = 1.1m, fullTextWeight = 1;
            var query = _DbSet as IQueryable<Post>;

            query = from p in query
                    let titleCount = search.Count(value => p.Title.Contains(value))
                    let titleMatch = titleCount > 0
                    let descriptionCount = search.Count(val => p.Description.Contains(val))
                    let descriptionMatch = descriptionCount > 0
                    let fullTextCount = search.Count(val => p.Content.Contains(val))
                    let fullTextMatch = fullTextCount > 0
                    let score = (titleMatch ? (titleCount * titleWeight) : 0) 
                                + 
                                (descriptionMatch ? (descriptionCount * descriptionWeight) : 0) 
                                + 
                                (fullTextMatch ? (fullTextCount * fullTextWeight) : 0)
                    where titleMatch || descriptionMatch || fullTextMatch
                    orderby score descending
                    select p;

            query = query
                .Include(o => o.PreviewImage)
                .Where(o => o.Status == Core.ContentStatus.Published)
                .Skip((page - 1) * perPage)
                .Take(perPage);

            return await query.ToListAsync();
        }

        public async Task<int> CountSearchAsync(string[] search)
        {
            var query = _DbSet.Where(o => o.Status == Core.ContentStatus.Published);

            query = query.Where(post =>
                search.Any(searchWord => post.Title.Contains(searchWord)) 
                || 
                search.Any(searchWord => post.Description.Contains(searchWord))
                ||
                search.Any(searchWord => post.Content.Contains(searchWord))
            );

            return await query.CountAsync();
        }

        public async Task<Post> GetRandomPostAsync()
        {
            double rnd = _Random.NextDouble();
            return await Repository
                .Query()
                .Filter(o => o.Status == Core.ContentStatus.Published)
                .OrderBy(o => o.OrderBy(f => SqlFunctions.Checksum(f.Id * rnd)))
                .FirstOrDefaultAsync();
        }
               
        //public async Task<IEnumerable<Post>> SearchPostsAsync(string[] search, int page, int perPage)
        //{
        //    const decimal titleWeight = 1.1m, descriptionWeight = 1;
        //    var query = _DbSet.Where(o => o.Status == Core.ContentStatus.Published);

        //    query = from p in query
        //            let titleMatch = search.Any(value => p.Title.Contains(value))
        //            let descriptionMatch = search.Any(val => p.Description.Contains(val))
        //            let score = (titleMatch ? titleWeight : 0) + (descriptionMatch ? descriptionWeight : 0)
        //            where titleMatch || descriptionMatch
        //            orderby score descending
        //            select p;

        //    query = query
        //        .Skip((page - 1) * perPage)
        //        .Take(perPage);

        //    return await query.ToListAsync();
        //}
    }
}
