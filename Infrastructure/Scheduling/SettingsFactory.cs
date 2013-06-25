using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Scheduling
{
    public static class SettingsFactory<T> where T : class
    {
        public static T CreateSettings(Dictionary<string, object> propers, T outType)
        {
            List<string> props = new List<string>();
            props.AddRange(GetPropertyNames(outType));
            Type t = outType.GetType();

            foreach (KeyValuePair<string, object> kvp in propers)
            {
                var s = (from p in props
                         where string.Equals(p, kvp.Key, StringComparison.OrdinalIgnoreCase)
                         select p).FirstOrDefault();
                if (s != null)
                {
                    PropertyInfo pi = t.GetProperty(s);
                    pi.SetValue(outType, Convert.ChangeType(kvp.Value, pi.PropertyType), null);
                }
            }
            return outType;
        }
        private static List<string> GetPropertyNames(T entity)
        {
            List<string> results = new List<string>();

            var properties = entity.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                results.Add(p.Name);
            }
            return results;

        }
        private static PropertyInfo ConvertProperty(PropertyInfo pi)
        {
            throw new NotImplementedException();
        }

    }
}
