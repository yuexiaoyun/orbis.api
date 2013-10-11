using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    public class DefaultObjectFactory : IObjectFactory
    {
        public object CreateObject(Type type, object argumentsAsAnonymousType)
        {
            object obj = null;

            if (argumentsAsAnonymousType != null)
            {
                var props = argumentsAsAnonymousType.GetType().GetProperties();

                obj = (from ctor in type.GetConstructors()
                       let parameters = ctor.GetParameters()
                       let hasSameParameters = (Func<bool>)(() =>
                       {
                           for (int index = parameters.Length - 1; index >= 0; --index)
                           {
                               var param = parameters[index];
                               var prop = props[index];

                               if (param.Name != prop.Name)
                               {
                                   return false;
                               }

                               if (param.ParameterType != prop.PropertyType)
                               {
                                   if (param.ParameterType.IsInterface &&
                                       prop.PropertyType.GetInterface(param.ParameterType.FullName) != null)
                                   {
                                       // argument property implements the interface in the ctor
                                       continue;
                                   }

                                   if (prop.PropertyType.IsSubclassOf(param.ParameterType))
                                   {
                                       // argument property derives the type in the ctor
                                       continue;
                                   }

                                   return false;
                               }
                           }

                           return true;
                       })
                       where parameters.Length == props.Length && hasSameParameters()
                       select Activator.CreateInstance(type, PropertiesToArray(argumentsAsAnonymousType))).SingleOrDefault();
            }

            else
            {
                obj = Activator.CreateInstance(type);
            }

            return obj;
        }

        public T CreateObject<T>(object argumentsAsAnonymousType)
        {
            return (T)CreateObject(typeof(T), argumentsAsAnonymousType);
        }

        private static object[] PropertiesToArray(object argumentsAsAnonymousType)
        {
            var objects = from p in argumentsAsAnonymousType.GetType().GetProperties()
                          select p.GetValue(argumentsAsAnonymousType);

            return objects.ToArray();
        }
    }
}
