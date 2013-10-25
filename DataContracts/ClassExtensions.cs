using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public static class ClassExtensions
    {
        public static T NotNull<T>(this T argument, string paramName)
            where T : class
        {
            if(argument == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        public static string NotNullOrEmpty(this string argument, string paramName)
        {
            if(string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(paramName);
            }

            return argument;
        }
    }
}
