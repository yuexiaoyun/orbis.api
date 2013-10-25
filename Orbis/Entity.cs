using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public abstract class Entity
    {
        public object Id { get; set; }
        public string PublicId { get; set; }
    }
}
