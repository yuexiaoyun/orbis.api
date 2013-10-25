using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Orbis.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnableETagAttribute : ActionFilterAttribute
    {
        public TimeSpan MaxAge { get; set; }
        public bool Caching { get; private set; }

        public EnableETagAttribute(bool caching)
        {
            Caching = caching;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if(request.Method == HttpMethod.Get)
            {
                var key = GetKey(request);
                var etagsFromClient = request.Headers.IfNoneMatch;

                if(etagsFromClient.Count > 0)
                {
                    EntityTagHeaderValue etag = null;

                    if(etags.TryGetValue(key, out etag) && etagsFromClient.Any(x => x.Tag == etag.Tag))
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotModified);

                        if(Caching)
                        {
                            SetCacheControl(actionContext.Response);
                        }
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var request = actionExecutedContext.Request;
            var key = GetKey(request);
            EntityTagHeaderValue etag;

            if(!etags.TryGetValue(key, out etag) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
            {
                etag = new EntityTagHeaderValue("\"" + Guid.NewGuid() + "\"");
                etags.AddOrUpdate(key, etag, (k, v) => etag);
            }

            actionExecutedContext.Response.Headers.ETag = etag;

            if(Caching)
            {
                SetCacheControl(actionExecutedContext.Response);
            }
        }

        private string GetKey(HttpRequestMessage request)
        {
            return request.RequestUri.ToString();
        }

        private void SetCacheControl(HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = MaxAge,
                MustRevalidate = false,
                Private = true
            };
        }

        private static readonly ConcurrentDictionary<string, EntityTagHeaderValue> etags = new ConcurrentDictionary<string, EntityTagHeaderValue>();
    }
}