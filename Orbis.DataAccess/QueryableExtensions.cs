using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> From<TEntity, TFilter>(this IQueryable<TEntity> queryable, TFilter filter)
            where TEntity : IEntity
            where TFilter : class, IFilter, new()
        {
            var queryBuilder = ContainerContext.Container.GetService<IQueryBuilder<TEntity, TFilter>>();

            if(queryBuilder == null)
            {
                throw new InvalidOperationException("No Query Builder registered.");
            }

            return queryBuilder.FillCriteria(queryable, filter);
        }
    }
}
