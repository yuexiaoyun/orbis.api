using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public abstract class QueryBuilder<TEntity, TFilter> : IQueryBuilder<TEntity, TFilter>
        where TEntity : IEntity
        where TFilter : class, IFilter, new()
    {
        public IQueryable<IEntity> FillCriteria(IQueryable<IEntity> queryable, IFilter filter)
        {
            return FillFilterCriteria(queryable as IQueryable<TEntity>, filter as TFilter) as IQueryable<IEntity>;
        }

        public IQueryable<TEntity> FillCriteria(IQueryable<TEntity> queryable, TFilter filter)
        {
            return FillFilterCriteria(queryable, filter);
        }

        protected abstract IQueryable<TEntity> FillFilterCriteria(IQueryable<TEntity> queryable, TFilter filter);
    }
}
