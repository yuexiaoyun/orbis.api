using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Web;
using System.Web.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Orbis.Api.Controllers;

namespace Orbis.Api.Handlers
{
    public abstract class BaseAuthenticationHandler : BaseHandler
    {
        protected BaseAuthenticationHandler()
        {
            var client = new MongoClient(WebConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            var server = client.GetServer();
            database = server.GetDatabase("test");

            AuthenticationHeaderParameters = null;
        }

        protected User GetUser(string username)
        {
            var collection = database.GetCollection<User>("users");
            var result = collection.FindOneAs<User>(Query<User>.EQ(x => x.Username, username));

            return result;
        }

        protected override HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, Exception exception)
        {
            var response = request.CreateResponse(HttpStatusCode.Unauthorized);
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(Scheme, AuthenticationHeaderParameters));

            if(exception is SecurityException)
            {
                response.ReasonPhrase = exception.Message;
            }

            return response;
        }

        protected override void Process(HttpResponseMessage response)
        {
            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(Scheme, AuthenticationHeaderParameters));
            }
        }

        protected abstract string Scheme { get; }
        protected virtual string AuthenticationHeaderParameters { get; private set; }

        private readonly MongoDatabase database;
    }
}