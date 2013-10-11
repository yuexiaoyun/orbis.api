using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace WcfModule
{
    public class ContainerProvider : IIoCContainerProvider
    {
        #region IIoCContainerProvider Members
        IIoCContainer IIoCContainerProvider.GetContainer()
        {
            IWindsorContainer windsorContainer = new WindsorContainer();
            windsorContainer.AddFacility<WcfFacility>()
                .Install(Configuration.FromAppConfig());

            return new WindsorContainerWrapper(windsorContainer);
        }
        #endregion
    }
}
