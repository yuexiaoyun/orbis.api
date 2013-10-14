using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Web;

namespace Orbis.Api
{
    public class AuthenticationManager : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if(incomingPrincipal == null || string.IsNullOrWhiteSpace(incomingPrincipal.Identity.Name))
            {
                throw new SecurityException("Name claim missing.");
            }

            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}