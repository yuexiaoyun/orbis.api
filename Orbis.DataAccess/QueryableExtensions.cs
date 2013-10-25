using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Orbis
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> From<TEntity, TFilter>(this IQueryable<TEntity> queryable, TFilter filter)
            where TEntity : Entity
            where TFilter : class, IFilter, new()
        {
            var dao = ContainerContext.Container.Resolve<IDao<TEntity, TFilter>>();

            if(dao == null)
            {
                throw new InvalidOperationException("No dao registered.");
            }

            return dao.FillCriteria(queryable, filter);
        }
    }
}
