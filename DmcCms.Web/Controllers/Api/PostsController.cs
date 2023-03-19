using Dmc.Cms.App;
using Dmc.Cms.Model;
using Dmc.Cms.Web.DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Dmc.Cms.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api")]
    public class PostsController : ApiController
    {
        // Shit shit shit ... but want to finish it.

        #region Private Fields

        private static readonly TimeSpan _HowLongToKeepListsInCache = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan _HowLongToKeepDetailsInCache = TimeSpan.FromMinutes(60);
        private readonly IPostService _PostService;
        private string _BaseUrl;

        #endregion

        #region Constructor

        public PostsController(IPostService postService)
        {
            _PostService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        #endregion

        #region Properties

        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_BaseUrl))
                {
                    _BaseUrl = new Uri(Request.RequestUri, RequestContext.VirtualPathRoot).GetLeftPart(UriPartial.Authority).ToString();
                }

                return _BaseUrl;
            }
        }

        #endregion

        #region API Methods

        [HttpGet]
        [Route("v1.0/posts")]
        public async Task<HttpResponseMessage> GetPosts([FromUri(Name = "page")] int? page = null, [FromUri(Name = "per_page")] int? perPage = null)
        {
            page = page ?? 1;
            perPage = perPage ?? int.MaxValue;
            string cacheKey = string.Format("App_{0}_Posts_Page_{1}_PerPage_{2}"
                , User.Identity.Name
                , page
                , perPage);

            PagedResult<PostDto> postsFromCache = CacheHelper.GetFromCache<PagedResult<PostDto>>(cacheKey);
            if (postsFromCache != null)
            {
                return CreateSuccesResult(postsFromCache);
            }

            int totalCount = await _PostService.CountAllPublishedAsync();
            if (totalCount <= 0)
            {
                return CreateEmptyListResult(page.Value, perPage.Value);
            }

            var posts = await _PostService.GetPagedPublishedAsync(page.Value, perPage.Value);
            if (posts == null)
            {
                return CreateEmptyListResult(page.Value, perPage.Value);
            }

            var result = CreatePostsSuccessResponse(page, perPage, totalCount, posts);
            CacheHelper.AddToCache(cacheKey, result, DateTimeOffset.UtcNow.Add(_HowLongToKeepListsInCache));
            var response = CreateSuccesResult(result);

            return response;
        }

        [HttpGet]
        [Route("v1.0/posts/{id:guid}")]
        public async Task<HttpResponseMessage> PostDetails(Guid? id)
        {
            if (!id.HasValue)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id is required.");
            }

            string cacheKey = string.Format("App_{0}_Post_{1}"
                , User.Identity.Name
                , id.Value);

            PostDetailDto postsFromCache = CacheHelper.GetFromCache<PostDetailDto>(cacheKey);
            if (postsFromCache != null)
            {
                return CreateSuccesApiResult(postsFromCache);
            }

            var post = await _PostService.FindByUniqueIdAsync(id.Value);
            if (post == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found.");
            }

            PostDetailDto postDetailDto = CreateDetailDto(post);
            if (postDetailDto == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occured.");
            }

            CacheHelper.AddToCache(cacheKey, postDetailDto, DateTimeOffset.UtcNow.Add(_HowLongToKeepDetailsInCache));
            return CreateSuccesApiResult(postDetailDto);
        }

        #endregion

        #region Private Methods

        private PagedResult<PostDto> CreatePostsSuccessResponse(int? page, int? perPage, int totalCount, IEnumerable<Post> posts)
        {
            List<PostDto> innerLIst = new List<PostDto>();

            foreach (var item in posts)
            {
                PostDto postDto = CreatePostDto(item);

                if (postDto != null)
                {
                    innerLIst.Add(postDto);
                }
            }

            var result = new PagedResult<PostDto>(innerLIst, page.Value, perPage.Value, totalCount)
            {
                Code = "000000"
            };

            result.Messages.Add("Success");

            return result;
        }

        // MOVE ALL THIS TO MAPPER!!!!

        private PostDetailDto CreateDetailDto(Post item)
        {
            var result = new PostDetailDto
            {
                Created = item.Created,
                Description = item.Description,
                Id = item.UniqueId,
                Title = item.Title,
                Updated = item.Modified,
                Link = CreateUrlToPost(item),
                Content = item.Content
            };

            if (item.PreviewImage != null)
            {
                result.PreviewImage = new ImageDto
                {
                    Thumbnail = !string.IsNullOrWhiteSpace(item.PreviewImage.SmallImage)
                        ? string.Concat(BaseUrl, item.PreviewImage.SmallImage)
                        : null,

                    LargeImage = !string.IsNullOrWhiteSpace(item.PreviewImage.LargeImage)
                        ? string.Concat(BaseUrl, item.PreviewImage.LargeImage)
                        : null,
                };
            }

            if (item.DetailImage != null)
            {
                result.DetailImage = new ImageDto
                {
                    Thumbnail = !string.IsNullOrWhiteSpace(item.DetailImage.SmallImage)
                        ? string.Concat(BaseUrl, item.DetailImage.SmallImage)
                        : null,

                    LargeImage = !string.IsNullOrWhiteSpace(item.DetailImage.LargeImage)
                        ? string.Concat(BaseUrl, item.DetailImage.LargeImage)
                        : null,
                };
            }

            AppendTagsIfPossible(item.Tags, result);
            AppendCategoriesIfPossible(item.Categories, result);

            return result;
        }

        private PostDto CreatePostDto(Post item)
        {
            var result = new PostDto
            {
                Created = item.Created,
                Description = item.Description,
                Id = item.UniqueId,
                Title = item.Title,
                Updated = item.Modified,
                Link = CreateUrlToPost(item),
            };

            if (item.PreviewImage != null)
            {
                result.PreviewImage = new ImageDto
                {
                    Thumbnail = !string.IsNullOrWhiteSpace(item.PreviewImage.SmallImage)
                        ? string.Concat(BaseUrl, item.PreviewImage.SmallImage)
                        : null,

                    LargeImage = !string.IsNullOrWhiteSpace(item.PreviewImage.LargeImage)
                        ? string.Concat(BaseUrl, item.PreviewImage.LargeImage)
                        : null,
                };
            }

            AppendTagsIfPossible(item.Tags, result);
            AppendCategoriesIfPossible(item.Categories, result);

            return result;
        }

        private void AppendCategoriesIfPossible(ICollection<Category> categories, PostDto result)
        {
            if (categories == null)
            {
                return;
            }

            foreach (var item in categories)
            {
                result.Categories.Add(new CategoryDto
                {
                    Id = item.UniqueId,
                    Title = item.Title
                });
            }
        }

        private void AppendTagsIfPossible(ICollection<Tag> tags, PostDto result)
        {
            if (tags == null)
            {
                return;
            }

            foreach (var item in tags)
            {
                result.Tags.Add(new TagDto
                {
                    Id = item.UniqueId,
                    Title = item.Title
                });
            }
        }

        private string CreateUrlToPost(Post item)
        {
            return Url.Link("PostDetails", new { Controller = "Default", Action = "Details", slug = item.Slug });
        }

        private HttpResponseMessage CreateSuccesApiResult(object postsFromCache)
        {
            ApiResult<object> apiResult = new ApiResult<object>
            {
                Code = "000000",
                Result = postsFromCache
            };

            apiResult.Messages.Add("Success");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(apiResult)
                    , Encoding.UTF8
                    , "application/json"
                ),
                ReasonPhrase = HttpStatusCode.OK.ToString(),
            };
        }

        private HttpResponseMessage CreateSuccesResult(object postsFromCache)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(postsFromCache)
                    , Encoding.UTF8
                    , "application/json"
                ),
                ReasonPhrase = HttpStatusCode.OK.ToString(),
            };
        }

        private HttpResponseMessage CreateEmptyListResult(int page, int perPage)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new PagedResult<object>(null, page, perPage, 0))
                    , Encoding.UTF8
                    , "application/json"
                ),
                ReasonPhrase = HttpStatusCode.OK.ToString(),
            };
        }

        #endregion

        #region Disposable

        bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_PostService != null)
                {
                    _PostService.Dispose();
                }
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }

        #endregion
    }
}
