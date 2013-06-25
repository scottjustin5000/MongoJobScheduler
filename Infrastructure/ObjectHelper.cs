using System;
using System.Reflection;

namespace Infrastructure
{
    public class ObjectHelper
    {
        private object _objRef;
        private Type _typeRef;

        public ObjectHelper(object o)
        {
            _objRef = o;
            _typeRef = o.GetType();
        }
        public bool HasProperty(string propertyName)
        {
            return (GetProperty(propertyName) != null);
        }

        public PropertyInfo GetProperty(string propertyName)
        {
            return _typeRef.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        }
        internal void SetValue(string propertyName, params object[] arguments)
        {
            try
            {

                PropertyInfo propertyInfo = _typeRef.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(_objRef, arguments[0], null);
                }
                else
                {
                    FieldInfo fieldInfo = _typeRef.GetField(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(_objRef, arguments[0]);
                    }
                }

            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                throw exception;
            }
        }
    }
}
