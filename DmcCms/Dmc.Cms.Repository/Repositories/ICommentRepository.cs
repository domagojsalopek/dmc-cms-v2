using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface ICommentRepository : IEntityRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllRepliesForCommentAsync(int id); 

        Task<bool> HasRepliesAsync(int id);

        Task<Comment> GetSingleForUserAndCommentIdAsync(Guid userId, int commentId); 

        Task<IEnumerable<Comment>> GetAllApprovedForPostAsync(int postId);

        Task<IEnumerable<Comment>> GetAllForUserAsync(Guid userId);
    }
}
