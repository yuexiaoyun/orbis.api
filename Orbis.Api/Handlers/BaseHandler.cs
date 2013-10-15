using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Orbis.Api.Handlers
{
    public abstract class BaseHandler : DelegatingHandler
    {
        protected override sealed async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Process(request);
                var response = await base.SendAsync(request, cancellationToken);
                Process(response);

                return response;
            }
            catch(Exception exception)
            {
                return CreateErrorResponse(request, exception);
            }
        }

        protected abstract void Process(HttpRequestMessage request);
        protected abstract void Process(HttpResponseMessage response);
        protected abstract HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, Exception exception);
    }
}