using Dmc.Cms.Model;
using Dmc.Repository;
using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal class CommentRepository : EntityRepositoryBase<Comment>, ICommentRepository
    {
        private readonly DbContext _DbContext; // THIS IS BEYOND CRAP ... 

        public CommentRepository(IRepository<Comment> repository, DbContext context) 
            : base(repository)
        {
            _DbContext = context;
        }

        public async Task<bool> HasRepliesAsync(int id)
        {
            return await Repository
                .Query()
                .Filter(o => o.ParentId == id) // this comment is a parent
                .CountAsync() > 0;
        }

        public async Task<IEnumerable<Comment>> GetAllRepliesForCommentAsync(int commentId)
        {
            string query = string.Format(@"
                ;with cte as 
                (
                    select Id from {0} where {1} = @CommentId AND {2} = 1
                    UNION ALL
                    select c.Id from cte inner join {0} c on cte.Id = c.{1} WHERE c.{2} = 1
                )
                select distinct Id from cte order by Id
            ", nameof (Comment)
             , nameof (Comment.ParentId)
             , nameof (Comment.Approved));

            List<int> ids = await _DbContext.Database
                .SqlQuery<int>(query, new SqlParameter("@CommentId", commentId))
                .ToListAsync();

            return await Repository.Query()
                .Filter(o => ids.Contains(o.Id))
                .GetEntitiesAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllApprovedForPostAsync(int postId)
        {
            return await Repository
                .Query()
                .Include(o => o.User)
                .Filter(o => 
                    o.PostId == postId
                    &&
                    o.Post.Status == Core.ContentStatus.Published // this doesn't belong here
                    &&
                    o.Approved
                    // here we also need marked as deleted because we need them to display the hierarchy
                )
                .GetEntitiesAsync();
        }

        public async Task<Comment> GetSingleForUserAndCommentIdAsync(Guid userId, int commentId)
        {
            return await Repository
                .Query()
                .Filter(o => 
                    o.User.UniqueId == userId 
                    && 
                    o.Id == commentId
                    &&
                    o.Status != CommentStatus.Deleted
                )
                .Include(o => o.Parent)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllForUserAsync(Guid userId)
        {
            return await Repository
                .Query()
                .Filter(o => 
                    o.User.UniqueId == userId
                    && 
                    o.Status != CommentStatus.Deleted // we should not show to user we still have something
                    &&
                    o.Post.Status == Core.ContentStatus.Published // is this ok?
                )
                .Include(o => o.Parent)
                .Include(o => o.Parent.User)
                .Include(o => o.Post)
                .OrderBy(o => o.OrderBy(x => x.Created))
                .GetEntitiesAsync();
        }
    }
}
