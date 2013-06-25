using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    static class ConfigurationFactory<T> where T : class
    {

        public static TDerived ToDerived<TBase, TDerived>(TBase tBase) where TDerived : TBase, new()
        {
            var tDerived = new TDerived();
            foreach (PropertyInfo propBase in typeof(TBase).GetProperties())
            {
                PropertyInfo propDerived = typeof(TDerived).GetProperty(propBase.Name);
                propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
            }
            return tDerived;
        }
        internal static T CreateTypedSettings(ScheduleSettings ss, T outType)
        {
            var props = new List<string>();
            props.AddRange(GetPropertyNames(outType));
            Type t = outType.GetType();

            foreach (KeyValuePair<string, object> kvp in ss.Properties)
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
    }
}
