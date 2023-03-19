using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ICommentService : ICrudService<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsHierarchyForPostAsync(int postId);

        Task<IEnumerable<Comment>> GetAllUserCommentsWithParentInfoAsync(Guid userId);

        Task<ServiceResult> DeleteUserCommentAsync(Guid userId, int commentId);
    }
}
