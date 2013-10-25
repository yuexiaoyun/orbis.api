using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IDao<T, in TFilter>
        where T : Entity
        where TFilter : IFilter
    {
        IQueryable<T> FillCriteria(IQueryable<T> queryable, TFilter filter);
    }
}
