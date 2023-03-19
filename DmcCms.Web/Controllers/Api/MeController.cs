using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Dmc.Cms.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api")]
    public class MeController : ApiController
    {
        [HttpGet]
        [Route("me")]
        public string Get()
        {
            return this.User.Identity.Name;
        }
    }
}
