using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Orbis.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RemoveXmlForGoogleChromeFilterAttribute : Attribute, IActionFilter
    {
        public bool AllowMultiple
        {
            get { return false; }
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var userAgent = actionContext.Request.Headers.UserAgent.ToString();

            if(userAgent.Contains("Chrome"))
            {
                var acceptHeaders = actionContext.Request.Headers.Accept;
                var header = acceptHeaders.SingleOrDefault(x => x.MediaType.Contains("application/xml"));
                acceptHeaders.Remove(header);
            }

            return await continuation();
        }
    }
}