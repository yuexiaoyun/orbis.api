using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IQueryBuilder
    {
        IQueryable<IEntity> FillCriteria(IQueryable<IEntity> queryable, IFilter filter);
    }

    public interface IQueryBuilder<TEntity, in TFilter> : IQueryBuilder
        where TEntity : IEntity
        where TFilter : class, IFilter, new()
    {
        IQueryable<TEntity> FillCriteria(IQueryable<TEntity> queryable, TFilter filter);
    }
}
