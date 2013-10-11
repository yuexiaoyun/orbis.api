using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IObjectFactory
    {
        object CreateObject(Type type, object argumentsAsAnonymousType = null);
        T CreateObject<T>(object argumentsAsAnonymousType = null);
    }
}
