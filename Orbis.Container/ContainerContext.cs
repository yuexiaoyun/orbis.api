using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Orbis
{
    public static class ContainerContext
    {
        public static IContainer Container { get; private set; }

        public static void Configure()
        {
            var provider = ApplicationConfiguration.GetContainerProvider(Assembly.GetCallingAssembly());

            if(provider != null)
            {
                Container = provider.GetContainer();
            }

            else
            {
                throw new ObjectNotDisposedException(Strings.ContainerContext_CannotConfigure, null);
            }
        }

        public static void Dispose()
        {
            var disposable = Container as IDisposable;

            if(disposable != null)
            {
                disposable.Dispose();
                Container = null;
            }

            else
            {
                throw new ObjectDisposedException("Container");
            }
        }

        #region Nested Definitions
        public static class Ambient
        {
            public static IContainer Container
            {
                get
                {
                    IContainer container = null;
                    var assembly = Assembly.GetCallingAssembly();

                    if(containers.ContainsKey(assembly))
                    {
                        container = containers[assembly];
                    }

                    return container;
                }
            }

            public static void Configure()
            {
                var thisAssembly = Assembly.GetAssembly(typeof(ContainerContext));
                var loadedAssemblies = GetAssemblies(AppDomain.CurrentDomain.BaseDirectory);

                var assemblies = (from loadedAssembly in loadedAssemblies
                                  let isReferenced = (from a in loadedAssembly.GetReferencedAssemblies()
                                                      where a.FullName == thisAssembly.FullName
                                                      select a).Any()
                                  where isReferenced
                                  select loadedAssembly).ToList();

                assemblies.Add(Assembly.GetCallingAssembly());

                foreach(var assembly in assemblies)
                {
                    if(!containers.ContainsKey(assembly))
                    {
                        var provider = ApplicationConfiguration.GetContainerProvider(assembly);

                        if(provider != null)
                        {
                            var container = provider.GetContainer();
                            containers.Add(assembly, container);
                        }
                    }

                    else
                    {
                        throw new ObjectNotDisposedException(Strings.ContainerContext_CannotConfigure, null);
                    }
                }
            }

            public static void Dispose()
            {
                var assembly = Assembly.GetCallingAssembly();

                if(containers.ContainsKey(assembly))
                {
                    var disposable = Container as IDisposable;

                    if(disposable != null)
                    {
                        disposable.Dispose();
                    }

                    containers.Remove(assembly);
                }

                else
                {
                    throw new ObjectDisposedException("Container");
                }
            }

            #region Private Members
            private static readonly IDictionary<Assembly, IContainer> containers = new Dictionary<Assembly, IContainer>();
            #endregion
        }
        #endregion

        #region Private Members
        private static IEnumerable<Assembly> GetAssemblies(string path)
        {
            var result = new List<Assembly>();

            var dir = new DirectoryInfo(path);

            if (dir.Exists)
            {
                var thisAssembly = Assembly.GetAssembly(typeof(ContainerContext));

                foreach(var file in dir.GetFiles("*.dll").Where(file => file.FullName != thisAssembly.Location))
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(file.FullName);
                        result.Add(asm);
                    }

                    catch(BadImageFormatException e)
                    {
                            
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
