using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Orbis
{
    internal sealed class WindsorContainerProvider : IContainerProvider
    {
        #region IContainerProvider Members
        void IContainerProvider.Initialize(string configuration)
        {
            windsorContainer = new WindsorContainer();

            if(configuration != null)
            {
                windsorContainer.Install(Configuration.FromXmlFile(configuration));
            }

            container = new WindsorContainerWrapper(windsorContainer);
        }

        IContainer IContainerProvider.GetContainer()
        {
            return container;
        }
        #endregion

        #region Private Members
        private IWindsorContainer windsorContainer;
        private IContainer container;
        #endregion
    }
}
