using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IContainer
    {
        string Name { get; }
        IContainer Parent { get; }

        T GetService<T>(string key = null, object argumentsAsAnonymousType = null);
        IEnumerable<T> GetAllServices<T>(object argumentsAsAnonymousType = null);

        object GetService(Type service, string key = null, object argumentsAsAnonymousType = null);
        Array GetAllServices(Type service, object argumentsAsAnonymousType = null);

        void RegisterService<T>(string key = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class;

        void RegisterService<I, T>(string key = null, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where I : class
            where T : class, I;

        void RegisterService(Type classType, Type serviceType = null, string key = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient);

        void InvokeServices<T>(Action<T> action);

        void AddChildContainer(IContainer container);
        void RemoveChildContainer(IContainer container);
        IContainer GetChildContainer(string name);
    }
}
