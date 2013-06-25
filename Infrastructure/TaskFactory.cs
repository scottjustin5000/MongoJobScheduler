using System;
using System.Collections.Generic;
using System.Reflection;


namespace Infrastructure
{
    public class Factory<T>
    {
        private Dictionary<string, Type> map = new Dictionary<string, Type>();

        public Factory()
        {
            Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();

            foreach (Type type in types)
            {
                if (!typeof(T).IsAssignableFrom(type) || type == typeof(T))
                {
                    continue;
                }
                map.Add(type.Name, type);
            }
        }

        public T CreateObject(string name, params object[] args)
        {
            Action<Exception> errorHandler = Try.HandleErrors.DefaultStrategy;
            object o = null;
            Try.Do(() => o = Activator.CreateInstance(map[name], args), error =>
            {
                errorHandler(error);
            });
            return (T)o;

        }
    }
}
