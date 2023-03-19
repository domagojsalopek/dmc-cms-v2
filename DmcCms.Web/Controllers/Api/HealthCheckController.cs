using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Dmc.Cms.Web.Controllers
{
    [RoutePrefix("api")]
    public class HealthCheckController : ApiController
    {
        [HttpGet]
        [Route("health_check")]
        public HttpResponseMessage HealthCheck()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK", Encoding.UTF8, "text/plain"),
                ReasonPhrase = "OK",
            };
        }
    }
}
