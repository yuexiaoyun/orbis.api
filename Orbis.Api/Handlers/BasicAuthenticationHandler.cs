using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Orbis.Api.Controllers;
using Orbis.Api.Extensions;

namespace Orbis.Api.Handlers
{
    public class BasicAuthenticationHandler : BaseAuthenticationHandler
    {
        protected override void Process(HttpRequestMessage request)
        {
            var headers = request.Headers;

            if(headers.Authorization != null && Scheme.Equals(headers.Authorization.Scheme))
            {
                var encoding = Encoding.UTF8;
                var credentials = encoding.GetString(Convert.FromBase64String(headers.Authorization.Parameter));
                var parts = credentials.Split(':');

                if(parts.Length != 2)
                {
                    throw new SecurityException("Authorization header with the scheme chosen is incorrect.");
                }

                var username = parts[0].Trim();
                var password = parts[1].Trim();
                var result = GetUser(username);

                if(result == null)
                {
                    throw new SecurityException("Invalid login.");
                }

                var hash = password.Md5Hash();

                if(hash != result.Password)
                {
                    throw new SecurityException("Invalid login.");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password) 
                };

                var identity = new ClaimsIdentity(claims, Scheme);
                var principal = new ClaimsPrincipal(identity);

                Thread.CurrentPrincipal = principal;

                if(HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
        }

        protected override string Scheme
        {
            get { return "Basic"; }
        }
    }
}