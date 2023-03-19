using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.DataTransferObjects
{
    public class PagedResult<T> : ApiResult
    {
        public PagedResult(IEnumerable<T> results, int page, int perPage, int totalCount)
        {
            Pagination = new PagedInfoDto
            {
                Page = page,
                PerPage = perPage,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)perPage)
            };

            Result = results;
        }

        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> Result
        {
            get;
            private set;
        }

        [JsonProperty(PropertyName = "pagination", NullValueHandling = NullValueHandling.Ignore)]
        public PagedInfoDto Pagination
        {
            get;
            private set;
        }
    }
}