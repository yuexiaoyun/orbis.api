using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Orbis.Api.Attributes;
using Orbis.Api.Extensions;

namespace Orbis.Api.Controllers
{
    [Authorize]
    [RemoveXmlForGoogleChromeFilter]
    public abstract class CrudController<TEntity, TContract> : ControllerBase
        where TEntity : Entity, new()
        where TContract : DataTransferObject, new()
    {
        // GET api/crud
        public async Task<TContract> Get(string id)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var result = collection.FindOneAs<TEntity>(Query<TEntity>.EQ(x => x.PublicId, id));
            var dto = result.As<TContract>();

            return dto;
        }

        // GET api/crud
        public async Task<IEnumerable<TContract>> GetAll()
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var items = collection
                .FindAllAs<TEntity>()
                .As<TContract>();

            return items;
        }

        // POST api/crud
        public async Task<TContract> Post([FromBody] TContract entity)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            entity.Id = Guid.NewGuid().ToString();
            OnCreating(entity);
            var obj = entity.As<TEntity>();
            obj.Id = ObjectId.GenerateNewId();
            collection.Insert(obj);

            return entity;
        }

        // PUT api/crud
        public async Task Put([FromBody] TContract entity)
        {
            var collection = Database.GetCollection<TEntity>(CollectionName);
            var item = collection.FindOneAs<TEntity>(Query<TEntity>.EQ(x => x.PublicId, entity.Id));

            var transformed = entity.As<TEntity>();
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

        #region Protected Members
        protected CrudController()
        {
            CollectionName = GetType().Name
                .SubstringUpToFirst("Controller")
                .ToLowerInvariant();
        }

        protected virtual void OnCreating(TContract dto)
        {

        }

        protected string CollectionName { get; private set; }
        #endregion
    }
}