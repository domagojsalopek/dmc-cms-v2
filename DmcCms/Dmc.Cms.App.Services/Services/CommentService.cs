using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Services
{
    public class CommentService : ServiceBase, ICommentService
    {
        #region Constructor

        public CommentService(ICmsUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region ICommentService

        public Task<int> CountAsync()
        {
            return UnitOfWork.CommentRepository.CountAsync();
        }

        public async Task<ServiceResult> DeleteAsync(Comment entity)
        {
            UnitOfWork.CommentRepository.Delete(entity);
            return await SaveAsync();
        }

        public Task<Comment> GetByIdAsync(int id)
        {
            return UnitOfWork.CommentRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Comment>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.CommentRepository.GetPagedAsync(page, perPage);
        }

        public Task<ServiceResult> InsertAsync(Comment entity)
        {
            UnitOfWork.CommentRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Comment entity)
        {
            UnitOfWork.CommentRepository.Update(entity);
            return SaveAsync();
        }

        public async Task<ServiceResult> DeleteUserCommentAsync(Guid userId, int commentId)
        {
            var comment = await UnitOfWork.CommentRepository.GetSingleForUserAndCommentIdAsync(userId, commentId);

            if (comment == null)
            {
                return ServiceResult.Succeeded; // nothing to do
            }

            //if (!await UnitOfWork.CommentRepository.HasRepliesAsync(commentId))
            //{

            //}

            comment.Status = CommentStatus.Deleted;
            comment.User = null;
            comment.UserId = null;
            comment.Text = string.Empty;
            comment.Author = null;

            return await SaveAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllUserCommentsWithParentInfoAsync(Guid userId)
        {
            return await UnitOfWork.CommentRepository.GetAllForUserAsync(userId);
        }

        public async Task<IEnumerable<Comment>> GetCommentAndAllRepliesHierarchy(int commentId)
        {
            Comment comment = await UnitOfWork.CommentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                return new List<Comment>();
            }

            IEnumerable<Comment> replies = await UnitOfWork.CommentRepository.GetAllRepliesForCommentAsync(commentId);

            if (replies == null)
            {
                return new List<Comment> { comment };
            }

            List<Comment> results = replies.ToList();

            results.Insert(0, comment);

            return CreateCommentHiearchy(results);
        }

        public async Task<IEnumerable<Comment>> GetCommentsHierarchyForPostAsync(int postId)
        {
            IEnumerable<Comment> comments = await UnitOfWork.CommentRepository.GetAllApprovedForPostAsync(postId);

            if (comments == null)
            {
                return new List<Comment>();
            }

            return CreateCommentHiearchy(comments);
        }

        #endregion

        #region Private Methods

        private List<Comment> CreateCommentHiearchy(IEnumerable<Comment> allCommentsFromDb)
        {
            List<Comment> result = new List<Comment>();
            IEnumerable<Comment> rootCategories = allCommentsFromDb.Where(o => o.IsRoot);

            foreach (Comment rootCategory in rootCategories)
            {
                rootCategory.Comments.Clear();
                FillChildren(rootCategory, allCommentsFromDb);

                // add filled to result
                result.Add(rootCategory);
            }

            return result
                .OrderBy(o => o.Created) // root by order posted ... ??
                .ToList();
        }

        private void FillChildren(Comment category, IEnumerable<Comment> allCategoriesFromDb)
        {
            var children = allCategoriesFromDb
                .Where(o => o.ParentId.HasValue && o.ParentId == category.Id)
                .OrderBy(o => o.Created); // replies are ordered asc ??

            category.Comments.Clear();
            foreach (var item in children)
            {
                FillChildren(item, allCategoriesFromDb);

                category.Comments.Add(item);
                item.Parent = category; // DO WE NEED THIS?
            }
        }

        #endregion
    }
}
