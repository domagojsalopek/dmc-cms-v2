using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Web.DataTransferObjects;
using Dmc.Cms.Web.Settings;
using Dmc.Cms.Web.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class CommentController : ControllerBase
    {
        private const string SuccessCode = "000000";
        private readonly ICommentService _CommentService;
        private const string DefaultAnonymousAuthorName = "Anonymous";

        public CommentController(ICommentService commentService, ApplicationUserManager manager, IAppConfig appConfig) 
            : base(manager, appConfig)
        {
            _CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        public async Task<ActionResult> LatestComments(int? id)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            IEnumerable<Comment> comments = await _CommentService.GetCommentsHierarchyForPostAsync(id.Value);
            List<CommentDto> commentDtos = CreateCommentListDto(comments);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new ApiResult<List<CommentDto>> { Code = SuccessCode, Result = commentDtos }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        public async Task<ActionResult> UserComments()
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            User user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            IEnumerable<Comment> comments = await _CommentService.GetAllUserCommentsWithParentInfoAsync(user.UniqueId);
            List<UserProfileCommentDto> commentDtos = CreateUserProfileCommentListDto(comments, user);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new ApiResult<List<UserProfileCommentDto>> { Code = SuccessCode, Result = commentDtos }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        private List<UserProfileCommentDto> CreateUserProfileCommentListDto(IEnumerable<Comment> comments, User user)
        {
            List<UserProfileCommentDto> commentDtos = new List<UserProfileCommentDto>();

            foreach (Comment comment in comments)
            {
                UserProfileCommentDto dto = CreateUserProfileCommentDto(comment, user);

                if (dto != null)
                {
                    commentDtos.Add(dto);
                }
            }

            return commentDtos;
        }

        private List<CommentDto> CreateCommentListDto(IEnumerable<Comment> comments)
        {
            List<CommentDto> commentDtos = new List<CommentDto>();

            foreach (Comment comment in comments)
            {
                CommentDto dto = CreateCommentDto(comment);

                if (dto != null)
                {
                    commentDtos.Add(dto);
                }
            }

            return commentDtos;
        }

        private CommentDto CreateCommentDto(Comment comment)
        {
            CommentDto result = CreateCommentBasics(comment);

            if (comment.Comments != null)
            {
                result.Replies = CreateCommentListDto(comment.Comments);
            }

            return result;
        }

        private static CommentDto CreateCommentBasics(Comment comment)
        {
            return new CommentDto
            {
                Author = ResolveCommentAuthorToReturn(comment),
                Date = comment.Created.ToUniversalTime(),
                Id = comment.Id,
                ParentAuthor = ResolveCommentAuthorToReturn(comment.Parent),
                ParentId = comment.ParentId,
                Status = comment.Status,
                Text = PrepareCommentResponseBody(comment),
                Replies = new List<CommentDto>()
            };
        }

        private static string PrepareCommentResponseBody(Comment comment)
        {
            if (comment.Status == CommentStatus.Deleted)
            {
                return null;
            }

            return Regex.Replace(comment.Text, "[\r\n]+", "<br/>");
        }

        private UserProfileCommentDto CreateUserProfileCommentDto(Comment comment, User user)
        {
            UserProfileCommentDto originalCOmment = CreateComment(comment, user);

            if (comment.Parent != null)
            {
                originalCOmment.Parent = CreateComment(comment.Parent, user);
                originalCOmment.Parent.Slug = originalCOmment.Slug;
            }

            return originalCOmment;
        }

        private static UserProfileCommentDto CreateComment(Comment comment, User user)
        {
            UserProfileCommentDto originalCOmment = new UserProfileCommentDto
            {
                Status = comment.Status,
                Author = ResolveCommentAuthorToReturn(comment),
                Date = comment.Created.ToUniversalTime(),
                Id = comment.Id,
                Text = PrepareCommentResponseBody(comment),
                Slug = comment.Post?.Slug,
                IsOwn = comment.UserId == user.Id
            };
            return originalCOmment;
        }

        [HttpPost, ValidateAntiForgeryToken] // horrific
        public async Task<ActionResult> AddComment(AddCommentViewModel model)
        {
            if (!Request.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return new HttpUnauthorizedResult();
            }

            var user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return new HttpUnauthorizedResult();
            }

            Comment originalComment = null;
            if (model.ReplyTo.HasValue)
            {
                originalComment = await _CommentService.GetByIdAsync(model.ReplyTo.Value);

                if (originalComment == null)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            if (string.IsNullOrWhiteSpace(model.Comment))
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new ApiResult<CommentDto>
                    {
                        Code = "000002"
                    }),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }

            string commentText = GetCommentText(model);

            if (string.IsNullOrWhiteSpace(commentText))
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new ApiResult<CommentDto>
                    {
                        Code = "000002"
                    }),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }

            Comment comment = new Comment
            {
                Approved = true, // for now
                Email = user.Email,
                ParentId = model.ReplyTo,
                Parent = originalComment,
                PostId = model.PostId.Value,
                Text = commentText,
                UserId = user.Id,
                Author = ResolveAuthorFromUser(user),
                Status = CommentStatus.Active
            };

            ServiceResult serviceResult = await _CommentService.InsertAsync(comment);

            if (!serviceResult.Success)
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new ApiResult<CommentDto>
                    {
                        Code = "000002"
                    }),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new ApiResult<CommentDto>
                {
                    Code = SuccessCode,
                    Result = CreateCommentBasics(comment)
                }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        private string GetCommentText(AddCommentViewModel model)
        {
            string temp = Regex.Replace(model.Comment.Trim(), "[\r\n]{2,}", Environment.NewLine);
            return Regex.Replace(temp, "[\r\n]+", new MatchEvaluator(HandleLineBreaks)); // should we do this??
        }

        private int _MatchNumber = 0;
        private const int MaxNumberOfLineBreaks = 10;

        private string HandleLineBreaks(Match match)
        {
            _MatchNumber++;

            if (_MatchNumber > MaxNumberOfLineBreaks) 
            {
                return " ";
            }

            return match.ToString();
        }

        [HttpPost, ValidateAntiForgeryToken] // we handle authorize inside as we don't want html back.
        public async Task<ActionResult> Delete(int? id)
        {
            if (!Request.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            if (!Request.IsAjaxRequest())
            {
                return new HttpUnauthorizedResult();
            }

            if (!id.HasValue || !HoneyPotCheckValid(EnvironmentKeys.HoneyPotFieldName))
            {
                return new HttpUnauthorizedResult();
            }

            var user = await GetLoggedInUserAsync();

            if (user == null)
            {
                return new HttpUnauthorizedResult();
            }

            ServiceResult deleteCommentResult = await _CommentService.DeleteUserCommentAsync(user.UniqueId, id.Value);

            if (!deleteCommentResult.Success)
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new ApiResult
                    {
                        Code = "000002"
                    }),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(new ApiResult
                {
                    Code = SuccessCode,
                }),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }

        // we should not even store it as we require that user is logged in. todo: make dependent on config
        private static string ResolveAuthorFromUser(User user)
        {
            if (!string.IsNullOrWhiteSpace(user?.NickName))
            {
                return user.NickName;
            }

            return !string.IsNullOrWhiteSpace(user.FullName)
                ? user.FullName
                : DefaultAnonymousAuthorName;
        }

        private static string ResolveCommentAuthorToReturn(Comment comment)
        {
            if (comment == null || comment.Status == CommentStatus.Deleted)
            {
                return DefaultAnonymousAuthorName;
            }

            if (comment.User == null) // user might be deleted
            {
                // return what was saved on the comment at the time of creating
                return !string.IsNullOrWhiteSpace(comment.Author)
                    ? comment.Author
                    : DefaultAnonymousAuthorName;
            }

            return ResolveAuthorFromUser(comment.User);
        }
    }
}