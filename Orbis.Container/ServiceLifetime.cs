using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    /// <summary>
    /// Specifies the lifetime of a service.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Single instance is maintained by the container.
        /// </summary>
        Singleton = 0,
        /// <summary>
        /// Single instance is maintained in a single thread.
        /// </summary>
        Thread = 1,
        /// <summary>
        /// An instance is created on each request.
        /// </summary>
        Transient = 2,
        /// <summary>
        /// Transient instances are pooled.
        /// </summary>
        Pooled = 3,
        /// <summary>
        /// Single instance is maintained in a single web request.
        /// </summary>
        PerWebRequest = 4
    }
}