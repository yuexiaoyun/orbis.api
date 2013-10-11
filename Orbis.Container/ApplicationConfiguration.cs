using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Orbis
{
    internal static class ApplicationConfiguration
    {
        public static IContainerProvider GetContainerProvider(Assembly callingAssembly)
        {
            IContainerProvider containerProvider = null;

            var codeBase = callingAssembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = uri.Uri.LocalPath + uri.Fragment;
            var dir = Path.GetDirectoryName(path);

            var configFile = dir + "\\" + callingAssembly.GetName().Name.ToLowerInvariant() + ".config";

            if (File.Exists(configFile))
            {
                var configuration = XDocument.Load(configFile);
                var providerElement = (from e in (configuration.FirstNode as XElement).Elements()
                                         where e.Name.LocalName == "containerProvider"
                                            && e.Attribute("type") != null && e.Attribute("reference") != null
                                         select e).SingleOrDefault();

                if(providerElement != null)
                {
                    var type = providerElement.Attribute("type").Value;

                    var containerProviderType = Type.GetType(type, true);
                    var containerProviderInterfaceName = typeof(IContainerProvider).FullName;
                    bool isImplementedByProvidedType = containerProviderType.GetInterface(containerProviderInterfaceName) != null;

                    if (!isImplementedByProvidedType)
                    {
                        var message = string.Format(Strings.ContainerProvider_DoesNotImplementRequiredInterface,
                                      ContainerProviderSection.ElementName, containerProviderInterfaceName);
                        throw new NotImplementedException(message);
                    }

                    var reference = providerElement.Attribute("reference").Value;

                    containerProvider = (IContainerProvider)Activator.CreateInstance(containerProviderType);
                    containerProvider.Initialize(reference);
                }
            }

            return containerProvider;
        }
    }
}
