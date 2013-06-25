using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
    /// <summary>
    /// Provides a collection of objects that is accessible by key or index.
    /// </summary>
    [Serializable]
    public class DynamicArrayList : DynamicArrayList<object>
    {
        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList() : base() { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(int capacity) : base(capacity) { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(IEqualityComparer comparer) : base((IEqualityComparer<object>)comparer) { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(int capacity, IEqualityComparer comparer) : base(capacity, (IEqualityComparer<object>)comparer) { }

        /// <summary>
        /// Creates a DynamicArrayList with the specified values.
        /// </summary>
        public DynamicArrayList(object[] values) : base(values) { }

        /// <summary>
        /// Creates a DynamicArrayList with the specified values.
        /// </summary>
        public DynamicArrayList(object[] values,IEqualityComparer comparer) : base(values, (IEqualityComparer<object>)comparer) { }

        /// <summary>
        /// Recreates a serialized DynamicArrayList.
        /// </summary>
        protected DynamicArrayList(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Provides a collection of objects that is accessible by index.
    /// </summary>
    [Serializable]
    public class DynamicArrayList<TValue> : ArrayListBase<TValue>, ISerializable
    {
        private const int DEFAULT_CAPACITY = 16;

        private List<TValue> _values;
        private ArrayListBase<TValue> _base;

        /// <summary>
        /// Creates a DynamicArrayList with the default initial capacity.
        /// </summary>
        public DynamicArrayList() : this(DEFAULT_CAPACITY, null) { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(int capacity) : this(new List<TValue>(capacity), null) { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(IEqualityComparer<TValue> comparer) : this(new List<TValue>(DEFAULT_CAPACITY), comparer) { }

        /// <summary>
        /// Creates a new instance of type DynamicArrayList.
        /// </summary>
        public DynamicArrayList(int capacity, IEqualityComparer<TValue> comparer) : this(new List<TValue>(capacity), comparer) { }

        /// <summary>
        /// Creates a DynamicArrayList with the specified values.
        /// </summary>
        public DynamicArrayList(TValue[] values) : this(values, null) { }

        /// <summary>
        /// Creates a DynamicArrayList with the specified values.
        /// </summary>
        public DynamicArrayList(TValue[] values, IEqualityComparer<TValue> comparer)
            : this((values != null ? values.Length : 0), comparer)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            Add(values);
        }

        /// <summary>
        /// Recreates a serialized DynamicArrayList.
        /// </summary>
        protected DynamicArrayList(SerializationInfo info, StreamingContext context)
            : base((IEqualityComparer<TValue>)info.GetValue("Comparer", typeof(IEqualityComparer<TValue>)))
        {
            if (info.GetBoolean("HasBase"))
            {
                _base = (ArrayListBase<TValue>)info.GetValue("BaseData", typeof(ArrayListBase<TValue>));
            }
            else
            {
                int count = info.GetInt32("Count");
                _values = new List<TValue>(count);

                for (int i = 0; i < count; i++)
                {
                    TValue value = (TValue)info.GetValue("V" + i.ToString(), typeof(TValue));
                    Add(value);
                }
            }
        }

        private DynamicArrayList(ArrayListBase<TValue> values, IEqualityComparer<TValue> comparer)
            : base(comparer)
        {
            _base = values;
        }

        private DynamicArrayList(List<TValue> values, IEqualityComparer<TValue> comparer)
            : base(comparer)
        {
            _values = values;
        }

        /// <summary>
        /// Creates and returns a read-only version of the specified 
        /// collection.
        /// </summary>
        public static DynamicArrayList<TValue> ReadOnly(DynamicArrayList<TValue> collection)
        {
            DynamicArrayList<TValue> readOnlyArrayList = null;

            if (collection.IsReadOnly)
            {
                readOnlyArrayList = collection;
            }
            else
            {
                readOnlyArrayList = new DynamicArrayList<TValue>(new ReadOnlyArrayList<TValue>(collection), collection.Comparer);
            }

            return readOnlyArrayList;
        }

        /// <summary>
        /// Creates and returns a fixed-size version of the specified 
        /// collection.
        /// </summary>
        public static DynamicArrayList<TValue> FixedSize(DynamicArrayList<TValue> collection)
        {
            DynamicArrayList<TValue> fixedSizeArrayList = null;

            if (collection.IsFixedSize)
            {
                fixedSizeArrayList = collection;
            }
            else
            {
                fixedSizeArrayList = new DynamicArrayList<TValue>(new FixedSizeArrayList<TValue>(collection), collection.Comparer);
            }

            return fixedSizeArrayList;
        }

        /// <summary>
        /// Creates and returns a synchronized version of the specified 
        /// collection.
        /// </summary>
        public static DynamicArrayList<TValue> Synchronized(DynamicArrayList<TValue> collection)
        {
            DynamicArrayList<TValue> synchronizedArrayList = null;

            if (collection.IsSynchronized)
            {
                synchronizedArrayList = collection;
            }
            else
            {
                synchronizedArrayList = new DynamicArrayList<TValue>(new SynchronizedArrayList<TValue>(collection), collection.Comparer);
            }

            return synchronizedArrayList;
        }

        /// <summary>
        /// Creates and returns a transactions version of the specified 
        /// collection.
        /// </summary>
        public static DynamicArrayList<TValue> Transactional(DynamicArrayList<TValue> collection)
        {
            DynamicArrayList<TValue> transactionalArrayList = null;

            if (collection.IsTransactional)
            {
                transactionalArrayList = collection;
            }
            else
            {
                transactionalArrayList = new DynamicArrayList<TValue>(new TransactionalArrayList<TValue>(collection), collection.Comparer);
            }

            return transactionalArrayList;
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public override int Count
        {
            get
            {
                int count = 0;

                if (_base != null)
                {
                    count = _base.Count;
                }
                else
                {
                    count = _values.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets a value that specifies if the collection is 
        /// read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                bool isReadOnly = false;

                if (_base != null)
                {
                    isReadOnly = _base.IsReadOnly;
                }

                return isReadOnly;
            }
        }

        /// <summary>
        /// Gets a value that specifies if the collection is 
        /// fixed-size.
        /// </summary>
        public override bool IsFixedSize
        {
            get
            {
                bool isFixedSize = false;

                if (_base != null)
                {
                    isFixedSize = _base.IsFixedSize;
                }

                return isFixedSize;
            }
        }

        /// <summary>
        /// Gets a value that specifies if the collection is 
        /// synchronized.
        /// </summary>
        public override bool IsSynchronized
        {
            get
            {
                bool isSynchronized = false;

                if (_base != null)
                {
                    isSynchronized = _base.IsSynchronized;
                }

                return isSynchronized;
            }
        }

        /// <summary>
        /// Gets a value that specifies if the collection is 
        /// transactional.
        /// </summary>
        public override bool IsTransactional
        {
            get
            {
                bool isTransactional = false;

                if (_base != null)
                {
                    isTransactional = _base.IsTransactional;
                }

                return isTransactional;
            }
        }

        /// <summary>
        /// Gets a value that can be used to synchronize access to
        /// the collection.
        /// </summary>
        public override object SyncRoot
        {
            get
            {
                object syncRoot = null;

                if (_base != null)
                {
                    syncRoot = _base.SyncRoot;
                }

                return syncRoot;
            }
        }

        /// <summary>
        /// Gets the index of the specified value.
        /// Returns -1 if the value does not exist 
        /// in the collection.
        /// </summary>
        /// <param name="value">
        /// The value whose index is desired.
        /// </param>
        public override int IndexOf(TValue value)
        {
            int index = -1;
            IEqualityComparer<TValue> comparer = Comparer;

            if (comparer != null)
            {
                if (_base != null)
                {
                    for (int i = 0; i < _base.Count; i++)
                    {
                        TValue compare = _base[i];

                        if (comparer.Equals(compare, value))
                        {
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _values.Count; i++)
                    {
                        TValue compare = _values[i];

                        if (comparer.Equals(compare, value))
                        {
                            index = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (_base != null)
                {
                    index = _base.IndexOf(value);
                }
                else
                {
                    index = _values.IndexOf(value);
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the values of the current collection in an 
        /// array. The values are in the same order in which they 
        /// exist in the collection.
        /// </summary>
        public override TValue[] ToArray()
        {
            TValue[] values = null;

            if (_base != null)
            {
                values = _base.ToArray();
            }
            else
            {
                values = _values.ToArray();
            }

            return values;
        }

        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="value">
        /// The value of the item.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        public override void Add(TValue value)
        {
            if (_base != null)
            {
                _base.Add(value);
            }
            else
            {
                _values.Add(value);
            }
        }

        /// <summary>
        /// Adds an array of items to the collection.
        /// </summary>
        /// <param name="values">
        /// The values to add to the collection.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        public override void Add(TValue[] values)
        {
            if (_base != null)
            {
                _base.Add(values);
            }
            else
            {
                if (values != null && values.Length != 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        Add(values[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a new item in the collection at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index at which the item should be added.
        /// </param>
        /// <param name="value">
        /// The value of the item.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the index is out-of-range.
        /// </exception>
        public override void Insert(int index, TValue value)
        {
            if (_base != null)
            {
                _base.Insert(index, value);
            }
            else
            {
                if (index < 0 || index > _values.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                _values.Insert(index, value);
            }
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="value">
        /// The item to remove.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the the specified does not exist in the collection.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        public override void Remove(TValue value)
        {
            if (_base != null)
            {
                _base.Remove(value);
            }
            else
            {
                int index = _values.IndexOf(value);

                if (index == -1)
                {
                    throw new ArgumentException("The specified item does not exist in the collection.");
                }

                _values.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="index">
        /// The index of the item to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the index is out-of-range.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        public override void RemoveAt(int index)
        {
            if (_base != null)
            {
                _base.RemoveAt(index);
            }
            else
            {
                _values.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the collection of all items.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is fixed-size.
        /// </exception>
        public override void Clear()
        {
            if (_base != null)
            {
                _base.Clear();
            }
            else
            {
                _values.Clear();
            }
        }

        /// <summary>
        /// Moves an item to another location in the collection.
        /// </summary>
        /// <param name="oldIndex">
        /// The current index of the item to move.
        /// </param>
        /// <param name="newIndex">
        /// The index to which the item should be moved.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the old or new index is out-of-range.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        public override void Move(int oldIndex, int newIndex)
        {
            if (_base != null)
            {
                _base.Move(oldIndex, newIndex);
            }
            else
            {
                if (oldIndex < 0 || oldIndex >= _values.Count)
                {
                    throw new ArgumentOutOfRangeException("oldIndex");
                }

                if (newIndex < 0 || newIndex >= _values.Count)
                {
                    throw new ArgumentOutOfRangeException("newIndex");
                }

                TValue value = _values[oldIndex];
                _values.RemoveAt(oldIndex);
                _values.Insert(newIndex, value);
            }
        }

        /// <summary>
        /// Swaps positions of two items.
        /// </summary>
        /// <param name="firstIndex">
        /// The index of the first item.
        /// </param>
        /// <param name="secondIndex">
        /// The index of the second item.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the firstIndex is out-of-range.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the secondIndex is out-of-range.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the collection is read-only.
        /// </exception>
        public override void Swap(int firstIndex, int secondIndex)
        {
            if (_base != null)
            {
                _base.Swap(firstIndex, secondIndex);
            }
            else
            {
                if (firstIndex < 0 || firstIndex >= _values.Count)
                {
                    throw new ArgumentOutOfRangeException("firstIndex");
                }

                if (secondIndex < 0 || secondIndex >= _values.Count)
                {
                    throw new ArgumentOutOfRangeException("secondIndex");
                }

                TValue value = _values[firstIndex];
                _values[firstIndex] = _values[secondIndex];
                _values[secondIndex] = value;
            }
        }

        /// <summary>
        /// Determines if the an item exists in the collection.
        /// </summary>
        /// <param name="value">
        /// The item whose existence is desired.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null.
        /// </exception>
        public override bool Contains(TValue value)
        {
            bool contains = false;
            IEqualityComparer<TValue> comparer =Comparer;

            if (comparer != null)
            {
                if (_base != null)
                {
                    foreach (TValue compare in _base)
                    {
                        if (comparer.Equals(compare, value))
                        {
                            contains = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (TValue compare in _values)
                    {
                        if (comparer.Equals(compare, value))
                        {
                            contains = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (_base != null)
                {
                    contains = _base.Contains(value);
                }
                else
                {
                    contains = _values.Contains(value);
                }
            }

            return contains;
        }

        /// <summary>
        /// Gets/sets an item in the collection.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the key is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if the set operation is called and the collection is read-only.
        /// </exception>
        public override TValue this[int index]
        {
            get
            {
                TValue value = default(TValue);

                if (_base != null)
                {
                    value = _base[index];
                }
                else
                {
                    if (index < 0 || index >= _values.Count)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    value = _values[index];
                }

                return value;
            }
            set
            {
                if (_base != null)
                {
                    _base[index] = value;
                }
                else
                {
                    if (index < 0 || index >= _values.Count)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    _values[index] = value;
                }
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_base != null)
            {
                info.AddValue("HasBase", true);
                info.AddValue("BaseData", _base);
            }
            else
            {
                info.AddValue("HasBase", false);
                int count = Count;
                info.AddValue("Count", count);
                info.AddValue("Comparer", Comparer);

                for (int i = 0; i < count; i++)
                {
                    info.AddValue("V" + i.ToString(), this[i]);
                }
            }
        }
    }
}
