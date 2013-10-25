using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public class UserDao : IDao<User, UserFilter>
    {
        public IQueryable<User> FillCriteria(IQueryable<User> queryable, UserFilter filter)
        {
            if(!string.IsNullOrEmpty(filter.PublicId))
            {
                var lower = filter.PublicId.ToLower();

                queryable = queryable.Where(x => x.PublicId.ToLower().Contains(lower));
            }

            return queryable;
        }
    }
}
