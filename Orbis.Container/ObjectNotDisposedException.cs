using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    [Serializable]
    public class ObjectNotDisposedException : InvalidOperationException
    {
        public ObjectNotDisposedException(string objectName) : base(objectName) {}
        public ObjectNotDisposedException(string message, Exception innerException) : base(message, innerException) { }
        protected ObjectNotDisposedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
