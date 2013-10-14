using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.Security.Claims;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Orbis.Api
{
    public class AuthHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "preslav")
            };

            var identity = new ClaimsIdentity(claims, "dummy");
            var principal = new ClaimsPrincipal(identity);
            var config = new IdentityConfiguration();
            var newPrincipal = config.ClaimsAuthenticationManager.Authenticate(request.RequestUri.ToString(), principal);

            Thread.CurrentPrincipal = newPrincipal;

            if(HttpContext.Current != null)
            {
                HttpContext.Current.User = newPrincipal;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}