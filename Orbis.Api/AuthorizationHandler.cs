using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Orbis.Api
{
    public class AuthorizationHandler : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            return base.CheckAccess(context);
        }
    }
}