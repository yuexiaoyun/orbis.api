using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using MongoDB.Driver;

namespace Orbis.Api.Controllers
{
    public abstract class ControllerBase : ApiController
    {
        protected ControllerBase()
        {
            Client = new MongoClient(WebConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
            Server = Client.GetServer();
            Database = Server.GetDatabase("test");
        }

        protected MongoClient Client { get; private set; }
        protected MongoServer Server { get; private set; }
        protected MongoDatabase Database { get; private set; }
    }
}