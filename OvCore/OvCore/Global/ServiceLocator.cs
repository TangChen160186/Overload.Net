using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvCore.OvCore.Global
{
    public static class ServiceLocator
    {
        private static Dictionary<int, object> _services = new Dictionary<int, object>();

        public static void Provide<T>(T service)
            where T : class
        {
            _services.Add(typeof(T).GetHashCode(), service);

        }
        public static T? Get<T>()
            where T : class
        {
            if (_services.ContainsKey(typeof(T).GetHashCode()))
            {
                return _services[typeof(T).GetHashCode()] as T;
            }

            return null;
        }

    }
}
