using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Oauth
{
    public class RefreshTokenProvider : AuthenticationTokenProvider
    {
        public RefreshTokenProvider()
        {
        }

        public override void Create(AuthenticationTokenCreateContext context)
        {
            var t = context.SerializeTicket();
            context.SetToken(t);
            base.Create(context);
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            // TODO: invalidate this token?

            context.DeserializeTicket(context.Token);



            base.Receive(context);
        }
    }
}