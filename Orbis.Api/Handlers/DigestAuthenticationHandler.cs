using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
    public class Nonce
    {
        public static string Generate()
        {
            var data = new byte[16];

            using(var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(data);
            }

            var nonce = data.Md5Hash();
            nonces.TryAdd(nonce, new Tuple<int, DateTime>(0, DateTime.UtcNow.AddMinutes(10)));

            return nonce;
        }

        public static bool IsValid(string nonce, string nonceCount)
        {
            Tuple<int, DateTime> cached = null;
            nonces.TryGetValue(nonce, out cached);

            if(cached != null)
            {
                if(int.Parse(nonceCount) > cached.Item1)
                {
                    if(cached.Item2 > DateTime.UtcNow)
                    {
                        nonces[nonce] = new Tuple<int, DateTime>(int.Parse(nonceCount), cached.Item2);

                        return true;
                    }
                }
            }

            return false;
        }

        private static readonly ConcurrentDictionary<string, Tuple<int, DateTime>> nonces = new ConcurrentDictionary<string, Tuple<int, DateTime>>(); 
    }
    
    public class Header
    {
        public string Username { get; set; }
        public string Realm { get; set; }
        public string Nonce { get; set; }
        public string Uri { get; set; }
        public string Nc { get; set; }
        public string Cnonce { get; set; }
        public string Response { get; set; }
        public string Method { get; set; }

        public static Header UnauthorizedResponseHeader
        {
            get
            {
                return new Header()
                {
                    Realm = "dkxga",
                    Nonce = Orbis.Api.Handlers.Nonce.Generate()
                };
            }
        }

        public Header()
        {
            
        }

        public Header(string header, string method)
        {
            var parameters = header
                .Replace("\"", string.Empty)
                .Split(',')
                .Select(x => x.Trim())
                .Select(x => new { Object = x, Index = x.IndexOf("=", System.StringComparison.InvariantCulture) })
                .Select(x => new KeyValuePair<string, string>(x.Object.Substring(0, x.Index), x.Object.Substring(x.Index + 1)))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach(var param in parameters)
            {
                switch(param.Key)
                {
                    case "username":
                        Username = param.Value;
                        break;
                    case "realm":
                        Realm = param.Value;
                        break;
                    case "nonce":
                        Nonce = param.Value;
                        break;
                    case "uri":
                        Uri = param.Value;
                        break;
                    case "nc":
                        Nc = param.Value;
                        break;
                    case "cnonce":
                        Cnonce = param.Value;
                        break;
                    case "response":
                        Response = param.Value;
                        break;
                    case "method":
                        Method = param.Value;
                        break;
                }
            }

            if(string.IsNullOrEmpty(Method))
            {
                Method = method;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("realm=\"{0}\"", Realm);
            sb.AppendFormat(", nonce=\"{0}\"", Nonce);
            sb.AppendFormat(", qop=\"{0}\"", "auth");

            return sb.ToString();
        }
    }

    public class DigestAuthenticationHandler : BaseAuthenticationHandler
    {
        protected override void Process(HttpRequestMessage request)
        {
            var headers = request.Headers;

            if(headers.Authorization != null && Scheme.Equals(headers.Authorization.Scheme))
            {
                var header = new Header(request.Headers.Authorization.Parameter, request.Method.Method);

                if(Nonce.IsValid(header.Nonce, header.Nc))
                {
                    var user = GetUser(header.Username);
                    var ha1 = string.Format("{0}:{1}:{2}", header.Username, header.Realm, user.Password).Md5Hash();
                    var ha2 = string.Format("{0}:{1}", header.Method, header.Uri).Md5Hash();
                    var computedResponse = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", ha1, header.Nonce, header.Nc, header.Cnonce, "auth", ha2).Md5Hash();

                    if(string.CompareOrdinal(header.Response, computedResponse) == 0)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, header.Username),
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
            }
        }

        protected override string Scheme
        {
            get { return "Digest"; }
        }

        protected override string AuthenticationHeaderParameters
        {
            get { return Header.UnauthorizedResponseHeader.ToString(); }
        }
    }
}