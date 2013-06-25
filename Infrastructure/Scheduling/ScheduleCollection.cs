using System;
using Infrastructure.Collections;

namespace Infrastructure.Scheduling
{
    [Serializable]
    public class ScheduleCollection : CustomDictionary<string, ScheduleBase>
    {
        /// <summary>
        /// Creates a new instance of type <see cref="ScheduleCollection"/>.
        /// </summary>
        public ScheduleCollection() : base(new DynamicDictionary<string, ScheduleBase>(StringComparer.OrdinalIgnoreCase)) { }

        private ScheduleCollection(DictionaryBase<string, ScheduleBase> collection) : base(collection) { }

        /// <summary>
        /// Creates and returns a read-only version of the specified 
        /// collection.
        /// </summary>
        public static ScheduleCollection ReadOnly(ScheduleCollection collection)
        {
            ScheduleCollection readOnlyCollection = null;

            if (collection.IsReadOnly)
            {
                readOnlyCollection = collection;
            }
            else
            {
                readOnlyCollection = new ScheduleCollection(DynamicDictionary<string, ScheduleBase>.ReadOnly((DynamicDictionary<string, ScheduleBase>)collection.Values));
            }

            return readOnlyCollection;
        }

        /// <summary>
        /// Creates and returns a fixed-size version of the specified 
        /// collection.
        /// </summary>
        public static ScheduleCollection FixedSize(ScheduleCollection collection)
        {
            ScheduleCollection fixedSizeCollection = null;

            if (collection.IsFixedSize)
            {
                fixedSizeCollection = collection;
            }
            else
            {
                fixedSizeCollection = new ScheduleCollection(DynamicDictionary<string, ScheduleBase>.FixedSize((DynamicDictionary<string, ScheduleBase>)collection.Values));
            }

            return fixedSizeCollection;
        }

        /// <summary>
        /// Creates and returns a read-only version of the specified 
        /// collection.
        /// </summary>
        public static ScheduleCollection Synchronized(ScheduleCollection collection)
        {
            ScheduleCollection synchronizedCollection = null;

            if (collection.IsSynchronized)
            {
                synchronizedCollection = collection;
            }
            else
            {
                synchronizedCollection = new ScheduleCollection(DynamicDictionary<string, ScheduleBase>.Synchronized((DynamicDictionary<string, ScheduleBase>)collection.Values));
            }

            return synchronizedCollection;
        }

        /// <summary>
        /// Returns the key of the specified item.
        /// </summary>
        public override string KeyOf(ScheduleBase value)
        {
            return value.Name;
        }
    }
}
