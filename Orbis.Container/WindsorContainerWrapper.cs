using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Windsor;
using Component = Castle.MicroKernel.Registration.Component;

namespace Orbis
{
    internal sealed class WindsorContainerWrapper : IContainer, IDisposable
    {
        public WindsorContainerWrapper(IWindsorContainer windsorContainer)
        {
            if(windsorContainer == null)
            {
                throw new ArgumentNullException("windsorContainer");
            }

            this.windsorContainer = windsorContainer;
            childContainers = new Dictionary<string, IContainer>();
        }

        #region IContainer Members
        string IContainer.Name { get { return windsorContainer.Name; } }
        IContainer IContainer.Parent { get { return parent; } }

        T IContainer.GetService<T>(string key, object argumentsAsAnonymousType)
        {
            T result = default(T);

            if(key == null && argumentsAsAnonymousType == null)
            {
                result = windsorContainer.Resolve<T>();
            }

            else if(key == null)
            {
                result = windsorContainer.Resolve<T>(argumentsAsAnonymousType);
            }

            else if(argumentsAsAnonymousType == null)
            {
                result = windsorContainer.Resolve<T>(key);
            }

            else
            {
                result = windsorContainer.Resolve<T>(key, argumentsAsAnonymousType);
            }

            return result;
        }

        IEnumerable<T> IContainer.GetAllServices<T>(object argumentsAsAnonymousType)
        {
            return windsorContainer.ResolveAll<T>(argumentsAsAnonymousType);
        }

        object IContainer.GetService(Type service, string key, object argumentsAsAnonymousType)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            return windsorContainer.Resolve(key, service, argumentsAsAnonymousType);
        }

        Array IContainer.GetAllServices(Type service, object argumentsAsAnonymousType)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            return windsorContainer.ResolveAll(service, argumentsAsAnonymousType);
        }

        void IContainer.RegisterService<T>(string key, ServiceLifetime lifetime)
        {
            var registration = Component.For<T>();

            if(!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key);
            }

            windsorContainer.Register(registration.LifeStyle.Is(GetLifestyle(lifetime)));
        }

        void IContainer.RegisterService<I, T>(string key, ServiceLifetime lifetime)
        {
            var registration = Component.For<I>().ImplementedBy<T>();

            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key);
            }

            windsorContainer.Register(registration.LifeStyle.Is(GetLifestyle(lifetime)));
        }

        void IContainer.RegisterService(Type classType, Type serviceType, string key, ServiceLifetime lifetime)
        {
            if(classType == null)
            {
                throw new ArgumentNullException("classType");
            }

            if (serviceType == null)
            {
                serviceType = classType;
            }

            var registration = Component.For(serviceType).ImplementedBy(classType);

            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key);
            }

            windsorContainer.Register(registration.LifeStyle.Is(GetLifestyle(lifetime)));
        }

        void IContainer.InvokeServices<T>(Action<T> action)
        {
            if(action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach(var service in ((IContainer) this).GetAllServices<T>())
            {
                action(service);
            }
        }

        void IContainer.AddChildContainer(IContainer container)
        {
            var wrapper = container as WindsorContainerWrapper;

            if(wrapper != null)
            {
                lock (syncRoot)
                {
                    windsorContainer.AddChildContainer(wrapper.windsorContainer);
                    wrapper.parent = this;
                    childContainers.Add(container.Name, container);
                }
            }
        }

        void IContainer.RemoveChildContainer(IContainer container)
        {
            var wrapper = container as WindsorContainerWrapper;

            if (wrapper != null)
            {
                lock (syncRoot)
                {
                    windsorContainer.RemoveChildContainer(wrapper.windsorContainer);
                    wrapper.parent = null;
                    childContainers.Remove(container.Name);
                }
            }
        }

        IContainer IContainer.GetChildContainer(string name)
        {
            IContainer result = null;

            lock (syncRoot)
            {
                if(childContainers.ContainsKey(name))
                {
                    result = childContainers[name];
                }
            }

            return result;
        }
        #endregion

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            windsorContainer.Dispose();
        }
        #endregion

        #region Private Members
        private LifestyleType GetLifestyle(ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    return LifestyleType.Singleton;
                case ServiceLifetime.Thread:
                    return LifestyleType.Thread;
                case ServiceLifetime.Transient:
                    return LifestyleType.Transient;
                case ServiceLifetime.Pooled:
                    return LifestyleType.Pooled;
                case ServiceLifetime.PerWebRequest:
                    return LifestyleType.PerWebRequest;
                default:
                    return LifestyleType.Undefined;
            }
        }

        private IContainer parent;
        private readonly object syncRoot = new object();
        private readonly IWindsorContainer windsorContainer;
        private readonly IDictionary<string, IContainer> childContainers;
        #endregion
    }
}
