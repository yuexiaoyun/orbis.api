using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}
