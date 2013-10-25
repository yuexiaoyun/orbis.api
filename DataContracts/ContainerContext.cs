using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Orbis
{
    public class ContainerContext
    {
        public static ContainerContext Current { get; private set; }
        public static IUnityContainer Container { get; private set; }

        public static ContainerContext Create(IUnityContainer container)
        {
            return Current ?? (Current = new ContainerContext(container));
        }

        #region Private Members
        private ContainerContext(IUnityContainer container)
        {
            Container = container.NotNull("container");
        }
        #endregion
    }
}
