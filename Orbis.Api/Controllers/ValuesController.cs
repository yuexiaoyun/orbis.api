using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Orbis.Api.Controllers
{
    public class User : IEntity
    {
        public object Id { get; set; }
        public string PublicId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }

    public abstract class DataTransferObject
    {
        public string Id { get; set; }
    }

    public class UserT : DataTransferObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }

    public class UserFilter : IFilter
    {
        public object Id { get; set; }
        public string PublicId { get; set; }
    }

    public interface IUserDao
    {
        User Get(object id);
        User Get(string publicId);
    }

    public class UserQueryBuilder : QueryBuilder<User, UserFilter>
    {
        protected override IQueryable<User> FillFilterCriteria(IQueryable<User> queryable, UserFilter filter)
        {
            if(!string.IsNullOrEmpty(filter.PublicId))
            {
                var lower = filter.PublicId.ToLower();

                queryable = queryable.Where(x => x.PublicId.ToLower().Contains(lower));
            }

            return queryable;
        }
    }

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

    public class AuthInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
  
    public class AuthController : ControllerBase
    {
        public async Task<HttpResponseMessage> Post([FromBody] AuthInfo info)
        {
            var collection = Database.GetCollection<User>("users");
            var result = collection.FindOneAs<User>(Query<User>.EQ(x => x.Username, info.Username));
            
            if(result == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("Wrong username"));
            }

            var hash = info.Password.Md5Hash();

            if(hash != result.Password)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("Wrong password"));
            }

            var token = Guid.NewGuid();

            var response = Request.CreateResponse<object>(HttpStatusCode.OK, null);
            response.Headers.AddCookies(new[] { new CookieHeaderValue("AuthToken", token.ToString()) });

            return response;
        }
    }

    [RemoveXmlForGoogleChromeFilter]
    public abstract class CrudController<TEntity, TContract> : ControllerBase
        where TEntity : class, IEntity, new()
        where TContract : DataTransferObject, new()
    {
        // GET api/crud
        public async Task<TContract> Get(string id)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var result = collection.FindOneAs<TEntity>(Query<TEntity>.EQ(x => x.PublicId, id));
            var dto = Mapper.Map<TEntity, TContract>(result);

            return dto;
        }

        // GET api/crud
        public async Task<IEnumerable<TContract>> GetAll()
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var items = collection
                .FindAllAs<TEntity>()
                .Select(Mapper.Map<TEntity, TContract>);

            return items;
        }

        // POST api/crud
        public async Task<TContract> Post([FromBody] TContract entity)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            entity.Id = Guid.NewGuid().ToString();
            OnCreating(entity);
            var obj = Mapper.Map<TContract, TEntity>(entity);
            obj.Id = ObjectId.GenerateNewId();
            collection.Insert(obj);

            return entity;
        }

        // PUT api/crud
        public async Task Put([FromBody] TContract entity)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var item = collection.FindOneAs<TEntity>(Query<TEntity>.EQ(x => x.PublicId, entity.Id));

            var transformed = Mapper.Map<TContract, TEntity>(entity);
            transformed.Id = item.Id;
            transformed.PublicId = item.PublicId;

            collection.Save(transformed);
        }

        // DELETE api/crud/id
        public async Task Delete(string id)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            collection.Remove(Query<TEntity>.EQ(x => x.PublicId, id));
        }

        protected virtual void OnCreating(TContract dto)
        {
            
        }

        protected abstract string CollectionName { get; }
    }

    public class UsersController : CrudController<User, UserT>
    {
        protected override void OnCreating(UserT dto)
        {
            dto.Password = dto.Password.Md5Hash();
        }

        protected override string CollectionName
        {
            get { return "users"; }
        }
    }

    public static class StringExtensions
    {
        public static string Md5Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();

            foreach(var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
    }
}