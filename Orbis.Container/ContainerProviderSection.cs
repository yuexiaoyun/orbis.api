using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public sealed class ContainerProviderSection : ConfigurationSection
    {
        public static readonly string ElementName = "containerProvider";

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("reference", IsRequired = false)]
        public string Reference
        {
            get { return (string)this["reference"]; }
            set { this["reference"] = value; }
        }
    }
}
