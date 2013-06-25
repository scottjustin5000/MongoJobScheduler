using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Infrastructure
{
    public static class Upcaster
    {

        /// <typeparam name="TBase">The type of the base class.</typeparam>
        /// <typeparam name="TDerived">The type of the derived class.</typeparam>
        /// <param name="tBase">The original object of type TBase.</param>
        /// <returns>The new object of type TDerived.</returns>
        public static TDerived ToDerived<TBase, TDerived>(TBase tBase)
            where TDerived : TBase, new()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return ToDerived<TBase, TDerived>(tBase, bindingFlags);
        }

        /// <typeparam name="TBase">The type of the base class.</typeparam>
        /// <typeparam name="TDerived">The type of the derived class.</typeparam>
        /// <param name="tBase">The original object of type TBase.</param>
        /// <param name="propertiesToIgnore">Will ignore properties that are named in this collection.</param>
        /// <returns>The new object of type TDerived.</returns>
        public static TDerived ToDerived<TBase, TDerived>(TBase tBase, ICollection<string> propertiesToIgnore)
            where TDerived : TBase, new()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return ToDerived<TBase, TDerived>(tBase, bindingFlags, propertiesToIgnore);
        }

        /// <typeparam name="TBase">The type of the base class.</typeparam>
        /// <typeparam name="TDerived">The type of the derived class.</typeparam>
        /// <param name="tBase">The original object of type TBase.</param>
        /// <param name="bindingFlags">Determines which properties are selected from TBase to be copied.</param>
        /// <returns>The new object of type TDerived.</returns>
        public static TDerived ToDerived<TBase, TDerived>(TBase tBase, BindingFlags bindingFlags)
            where TDerived : TBase, new()
        {
            return ToDerived<TBase, TDerived>(tBase, bindingFlags, null);
        }
        /// <typeparam name="TBase">The type of the base class.</typeparam>
        /// <typeparam name="TDerived">The type of the derived class.</typeparam>
        /// <param name="tBase">The original object of type TBase.</param>
        /// <param name="bindingFlags">Determines which properties are selected from TBase to be copied.</param>
        /// <param name="propertiesToIgnore">Will ignore properties that are named in this collection.</param>
        /// <returns>The new object of type TDerived.</returns>
        public static TDerived ToDerived<TBase, TDerived>(TBase tBase, BindingFlags bindingFlags, ICollection<string> propertiesToIgnore)
            where TDerived : TBase, new()
        {
            if (tBase == null)
            {
                throw new ArgumentNullException("tBase");
            }

            bool allowNonPublic = ((bindingFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);

            TDerived tDerived = new TDerived();

            var baseProperties =
                from propBase in typeof(TBase).GetProperties(bindingFlags)
                where ((propertiesToIgnore == null) || (!propertiesToIgnore.Contains(propBase.Name)))
                where (propBase.GetGetMethod(allowNonPublic) != null)
                select propBase;

            foreach (PropertyInfo propBase in baseProperties)
            {
                PropertyInfo propDerived = typeof(TDerived).GetProperty(propBase.Name, bindingFlags, null,
                    propBase.PropertyType, new Type[0], null);

                // Only set the value on a non-indexed property.  An indexed property is more like a method than a property,
                // so there is no way to assign each of the values from the base to the derived type.
                if (
                    (propDerived != null)
                    && (propDerived.GetSetMethod(allowNonPublic) != null)
                    && (propBase.GetIndexParameters().Length == 0)
                    && (propDerived.GetIndexParameters().Length == 0)
                    )
                {
                    propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
                }
            }
            return tDerived;
        }
    }
}
