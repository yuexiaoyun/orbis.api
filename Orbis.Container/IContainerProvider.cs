using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public interface IContainerProvider
    {
        void Initialize(string configuration = null);
        IContainer GetContainer();
    }
}
