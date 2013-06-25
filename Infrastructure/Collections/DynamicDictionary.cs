using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides a collection of objects that is accessible by key or index.
   /// </summary>
   [Serializable]
   public class DynamicDictionary : DynamicDictionary<object, object> 
   {
      /// <summary>
      /// Creates a DynamicDictionary with the default initial capacity.
      /// </summary>
      public DynamicDictionary () {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// default initial capacity.
      /// </summary>
      public DynamicDictionary (int capacity) : base(capacity) {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// default initial capacity.
      /// </summary>
      public DynamicDictionary (IEqualityComparer<object> comparer) : base(comparer) {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// initial capacity.
      /// </summary>
      public DynamicDictionary (int capacity, IEqualityComparer<object> comparer) : base(capacity, comparer) {}
      
      /// <summary>
      /// Creates a DynamicArrayList with the specified keys and values.
      /// </summary>
      public DynamicDictionary (object[] keys, object[] values) : base(keys, values) {}
      
      /// <summary>
      /// Creates a DynamicArrayList with the specified keys and values.
      /// </summary>
      public DynamicDictionary (object[] keys, object[] values, IEqualityComparer<object> comparer) : base(keys, values, comparer) {}
      
      /// <summary>
      /// Recreates a serialized DynamicDictionary.
      /// </summary>
      protected DynamicDictionary (SerializationInfo info, StreamingContext context) : base(info, context) {}
   }

   /// <summary>
   /// Provides a collection of objects that is accessible by key or index.
   /// </summary>
   [Serializable]
   public class DynamicDictionary<TKey, TValue> : KeyedDictionary<TKey, TValue>, ISerializable, IDictionaryWrapper<TKey, TValue>
   {
      private const int DEFAULT_CAPACITY = 16;

      private List<TKey> _keys;
      private Dictionary<TKey, TValue> _values;
      private DictionaryBase<TKey, TValue> _base;

      /// <summary>
      /// Creates a case-sensitive DynamicDictionary with the default initial
      /// capacity.
      /// </summary>
      public DynamicDictionary () : this(DEFAULT_CAPACITY) {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// default initial capacity.
      /// </summary>
      public DynamicDictionary (int capacity) : this(new List<TKey>(capacity), new Dictionary<TKey, TValue>(capacity)) {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// default initial capacity.
      /// </summary>
      public DynamicDictionary (IEqualityComparer<TKey> comparer) : this(DEFAULT_CAPACITY, comparer) {}

      /// <summary>
      /// Creates a DynamicDictionary with the specified key comparer and 
      /// initial capacity.
      /// </summary>
      public DynamicDictionary (int capacity, IEqualityComparer<TKey> comparer) : this(new List<TKey>(capacity), new Dictionary<TKey, TValue>(capacity, comparer)) {}
      
      /// <summary>
      /// Creates a DynamicArrayList with the specified keys and values.
      /// </summary>
      public DynamicDictionary (TKey[] keys, TValue[] values) : this((keys != null ? keys.Length : 0)) 
      {
         Initialize(keys, values);
      }
      
      /// <summary>
      /// Creates a DynamicArrayList with the specified keys and values.
      /// </summary>
      public DynamicDictionary (TKey[] keys, TValue[] values, IEqualityComparer<TKey> comparer) : this((keys != null ? keys.Length : 0), comparer) 
      {
         Initialize(keys, values);
      }
      
      /// <summary>
      /// Recreates a serialized DynamicDictionary.
      /// </summary>
      protected DynamicDictionary (SerializationInfo info, StreamingContext context)
      {
         if (info.GetBoolean("HasBase"))
         {
            _base = (DictionaryBase<TKey, TValue>)info.GetValue("BaseData", typeof(DictionaryBase<TKey, TValue>));
         }
         else
         {
            int count = info.GetInt32("Count");
            IEqualityComparer<TKey> comparer = (IEqualityComparer<TKey>)info.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
            _keys = new List<TKey>(count);
            _values = new Dictionary<TKey, TValue>(count, comparer);
            bool simpleKey = info.GetBoolean("SimpleKey");

            if (simpleKey)
            {
               TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(TKey));
               SerializationInfoEnumerator enumerator = info.GetEnumerator();
               enumerator.MoveNext(); // HasBase
               enumerator.MoveNext(); // Count
               enumerator.MoveNext(); // Comparer
               enumerator.MoveNext(); // SimpleKey

               while (enumerator.MoveNext())
               {
                  TKey key = (TKey)typeConverter.ConvertFromString(enumerator.Name);
                  TValue value = (TValue)enumerator.Value;
                  BaseAdd(key, value);
               }
            }
            else
            {
               for (int i = 0; i < count; i++)
               {
                  TKey key = (TKey)info.GetValue("K" + i.ToString(), typeof(TKey));
                  TValue value = (TValue)info.GetValue("V" + i.ToString(), typeof(TValue));
                  BaseAdd(key, value);
               }
            }
         }
      }

      private DynamicDictionary (DictionaryBase<TKey, TValue> values)
      {
         _base = values;
      }

      private DynamicDictionary (List<TKey> keys, Dictionary<TKey, TValue> values)
      {
         _keys = keys;
         _values = values;
      }

      /// <summary>
      /// Creates and returns a read-only version of the specified 
      /// collection.
      /// </summary>
      public static DynamicDictionary<TKey, TValue> ReadOnly (DynamicDictionary<TKey, TValue> collection)
      {
         DynamicDictionary<TKey, TValue> readOnlyCollection = null;

         if (collection.IsReadOnly)
         {
            readOnlyCollection = collection;
         }
         else
         {
            readOnlyCollection = new DynamicDictionary<TKey, TValue>(new ReadOnlyDictionary<TKey, TValue>(collection));
         }

         return readOnlyCollection;
      }

      /// <summary>
      /// Creates and returns a fixed-size version of the specified 
      /// collection.
      /// </summary>
      public static DynamicDictionary<TKey, TValue> FixedSize (DynamicDictionary<TKey, TValue> collection)
      {
         DynamicDictionary<TKey, TValue> fixedSizeCollection = null;

         if (collection.IsFixedSize)
         {
            fixedSizeCollection = collection;
         }
         else
         {
            fixedSizeCollection = new DynamicDictionary<TKey, TValue>(new FixedSizeDictionary<TKey, TValue>(collection));
         }

         return fixedSizeCollection;
      }

      /// <summary>
      /// Creates and returns a synchronized version of the specified 
      /// collection.
      /// </summary>
      public static DynamicDictionary<TKey, TValue> Synchronized (DynamicDictionary<TKey, TValue> collection)
      {
         DynamicDictionary<TKey, TValue> synchronizedCollection = null;

         if (collection.IsSynchronized)
         {
            synchronizedCollection = collection;
         }
         else
         {
            synchronizedCollection = new DynamicDictionary<TKey, TValue>(new SynchronizedDictionary<TKey, TValue>(collection));
         }

         return synchronizedCollection;
      }

      /// <summary>
      /// Creates and returns a transactions version of the specified 
      /// collection.
      /// </summary>
      public static DynamicDictionary<TKey, TValue> Transactional (DynamicDictionary<TKey, TValue> collection)
      {
         DynamicDictionary<TKey, TValue> transactionalCollection = null;

         if (collection.IsTransactional)
         {
            transactionalCollection = collection;
         }
         else
         {
            transactionalCollection = new DynamicDictionary<TKey, TValue>(new TransactionalDictionary<TKey, TValue>(collection));
         }

         return transactionalCollection;
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
               count = _keys.Count;
            }

            return count;
         }
      }

      private void Initialize (TKey[] keys, TValue[] values)
      {
         if (keys == null)
         {
            throw new ArgumentNullException("keys");
         }

         if (values == null)
         {
            throw new ArgumentNullException("values");
         }

         if (keys.Length != values.Length)
         {
            throw new ArgumentException("The specified keys and values must be the same length.");
         }

         for (int i = 0; i < keys.Length; i++)
         {
            Add(keys[i], values[i]);
         }
      }

      /// <summary>
      /// Gets the key comparer for comparisons.
      /// </summary>
      public override IEqualityComparer<TKey> Comparer
      {
         get
         {
            IEqualityComparer<TKey> comparer = null;

            if (_base != null)
            {
               comparer = _base.Comparer;
            }
            else
            {
               comparer = _values.Comparer;
            }

            return comparer;
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
      /// Returns the key of the specified item, or null if the item does not exist in the collection.
      /// </summary>
      /// <param name="value">
      /// The value whose key is desired.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      public override TKey KeyOf (TValue value)
      {
         TKey key = default(TKey);

         if (_base != null)
         {
            key = _base.KeyOf(value);
         }
         else
         {
            foreach (KeyValuePair<TKey, TValue> entry in _values)
            {
               if (value != null)
               {
                  if (entry.Value != null)
                  {
                     if (entry.Value.Equals(value))
                     {
                        key = (TKey)entry.Key;
                        break;
                     }
                  }
               }
               else
               {
                  if (entry.Value == null)
                  {
                     key = (TKey)entry.Key;
                     break;
                  }
               }
            }
         }

         return key;
      }

      /// <summary>
      /// Gets the key of the item at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item whose key is desired.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      public override TKey KeyAt (int index)
      {
         TKey key = default(TKey);

         if (_base != null)
         {
            key = _base.KeyAt(index);
         }
         else
         {
            key = _keys[index];
         }

         return key;
      }

      /// <summary>
      /// Gets the index of the specified value.
      /// Returns -1 if the value does not exist 
      /// in the collection.
      /// </summary>
      /// <param name="value">
      /// The value whose index is desired.
      /// </param>
      public override int IndexOf (TValue value)
      {
         int index = -1;

         if (_base != null)
         {
            index = _base.IndexOf(value);
         }
         else
         {
            TKey key = KeyOf(value);

            if (key != null)
            {
               index = _keys.IndexOf(key);
            }
         }

         return index;
      }

      /// <summary>
      /// Gets the index of the item with the specified key.
      /// Returns -1 if the key does not exist in the collection.
      /// </summary>
      /// <param name="key">
      /// The key of the item whose index is desired.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      public override int GetIndex (TKey key)
      {
         int index = -1;

         if (_base != null)
         {
            index = _base.GetIndex(key);
         }
         else
         {
            if (key == null)
            {
               throw new ArgumentNullException("key");
            }

            index = _keys.IndexOf(key);
         }

         return index;
      }

      /// <summary>
      /// Returns the keys for the current collection.
      /// </summary>
      public override TKey[] GetKeys ()
      {
         TKey[] keys = null;

         if (_base != null)
         {
            keys = _base.GetKeys();
         }
         else
         {
            keys = _keys.ToArray();
         }

         return keys;
      }

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      public override TValue[] ToArray ()
      {
         TValue[] values = null;

         if (_base != null)
         {
            values = _base.ToArray();
         }
         else
         {
            values = new TValue[_keys.Count];

            for (int i = 0; i < values.Length; i++)
            {
               values[i] = _values[_keys[i]];
            }
         }

         return values;
      }

      /// <summary>
      /// Adds a new item to the collection.
      /// </summary>
      /// <param name="key">
      /// The key of the item.
      /// </param>
      /// <param name="value">
      /// The value of the item.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if an item exists in the collection with the same key.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      protected internal override void BaseAdd (TKey key, TValue value)
      {
         if (_base != null)
         {
            _base.BaseAdd(key, value);
         }
         else
         {
            if (key == null)
            {
               throw new ArgumentNullException("key");
            }

            if (_keys.Contains(key))
            {
               throw new ArgumentException("An item with key \"" + key.ToString() + "\" already exists in the collection.");
            }

            _keys.Add(key);
            _values.Add(key, value);
         }
      }

      /// <summary>
      /// Inserts a new item in the collection at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index at which the item should be added.
      /// </param>
      /// <param name="key">
      /// The key of the item.
      /// </param>
      /// <param name="value">
      /// The value of the item.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if an item exists in the collection with the same key.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      protected internal override void BaseInsert (int index, TKey key, TValue value)
      {
         if (_base != null)
         {
            _base.BaseInsert(index, key, value);
         }
         else
         {
            if (index < 0 || index > _keys.Count)
            {
               throw new ArgumentOutOfRangeException("index");
            }

            if (key == null)
            {
               throw new ArgumentNullException("key");
            }

            if (_keys.Contains(key))
            {
               throw new ArgumentException("An item with key \"" + key.ToString() + "\" already exists in the collection.");
            }

            _keys.Insert(index, key);
            _values.Add(key, value);
         }
      }

      /// <summary>
      /// Removes an item from the collection.
      /// </summary>
      /// <param name="key">
      /// The key of the item to remove.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the an item with the specified key does not exist 
      /// in the collection.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      public override void Remove (TKey key)
      {
         if (_base != null)
         {
            _base.Remove(key);
         }
         else
         {
            if (key == null)
            {
               throw new ArgumentNullException("key");
            }

            if (!_keys.Contains(key))
            {
               throw new ArgumentException("An item with key \"" + key.ToString() + "\" does not exist in the collection.");
            }

            _keys.Remove(key);
            _values.Remove(key);
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
      public override void RemoveAt (int index)
      {
         if (_base != null)
         {
            _base.RemoveAt(index);
         }
         else
         {
            if (index < 0 || index >= _keys.Count)
            {
               throw new ArgumentOutOfRangeException("index");
            }

            TKey key = _keys[index];
            _keys.RemoveAt(index);
            _values.Remove(key);
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
      public override void Clear ()
      {
         if (_base != null)
         {
            _base.Clear();
         }
         else
         {
            _keys.Clear();
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
      public override void Move (int oldIndex, int newIndex)
      {
         if (_base != null)
         {
            _base.Move(oldIndex, newIndex);
         }
         else
         {
            if (oldIndex < 0 || oldIndex >= _keys.Count)
            {
               throw new ArgumentOutOfRangeException("oldIndex");
            }

            if (newIndex < 0 || newIndex >= _keys.Count)
            {
               throw new ArgumentOutOfRangeException("newIndex");
            }

            TKey key = _keys[oldIndex];
            _keys.RemoveAt(oldIndex);
            _keys.Insert(newIndex, key);
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
      public override void Swap (int firstIndex, int secondIndex)
      {
         if (_base != null)
         {
            _base.Swap(firstIndex, secondIndex);
         }
         else
         {
            if (firstIndex < 0 || firstIndex >= Count)
            {
               throw new ArgumentOutOfRangeException("firstIndex");
            }

            if (secondIndex < 0 || secondIndex >= Count)
            {
               throw new ArgumentOutOfRangeException("secondIndex");
            }

            TKey firstKey = _keys[firstIndex];
            TKey secondKey = _keys[secondIndex];
            TKey tempKey = _keys[firstIndex];
            _keys[firstIndex] = secondKey;
            _keys[secondIndex] = tempKey;
         }
      }

      /// <summary>
      /// Determines if the an item with the specified key exists in the
      /// collection.
      /// </summary>
      /// <param name="key">
      /// The key of the item whose existence is desired.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      public override bool HasKey (TKey key)
      {
         bool contains = false;

         if (_base != null)
         {
            contains = _base.HasKey(key);
         }
         else
         {
            contains = _values.ContainsKey(key);
         }

         return contains;
      }

      /// <summary>
      /// Determines if the an item exists in the collection.
      /// </summary>
      /// <param name="value">
      /// The item whose existence is desired.
      /// </param>
      public override bool Contains (TValue value)
      {
         bool hasValue = false;

         if (_base != null)
         {
            hasValue = _base.Contains(value);
         }
         else
         {
            hasValue = _values.ContainsValue(value);
         }

         return hasValue;
      }

      /// <summary>
      /// Sets the key for an item.
      /// </summary>
      /// <param name="index">
      /// The index of the item.
      /// </param>
      /// <param name="newKey">
      /// The new key of the item.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the the new key is null.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the an item already exists in the 
      /// collection with the specified new key.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      public override void SetKey (int index, TKey newKey)
      {
         if (_base != null)
         {
            _base.SetKey(index, newKey);
         }
         else
         {
            if (index < 0 || index >= _values.Count)
            {
               throw new ArgumentOutOfRangeException("index");
            }

            if (_keys.Contains(newKey))
            {
               throw new ArgumentException("An item with key \"" + newKey.ToString() + "\" already exists in the dictionary.");
            }

            TKey key = _keys[index];
            TValue value = _values[key];
            _keys[index] = newKey;
            _values.Remove(key);
            _values.Add(newKey, value);
         }
      }

      /// <summary>
      /// Returns the value at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      public override TValue ValueAt (int index)
      {
         TValue value = default(TValue);

         if (_base != null)
         {
            value = _base.ValueAt(index);
         }
         else
         {
            if (index < 0 || index >= _values.Count)
            {
               throw new ArgumentOutOfRangeException("index");
            }

            value = _values[_keys[index]];
         }

         return value;
      }

      /// <summary>
      /// Sets the value at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item.
      /// </param>
      /// <param name="value">
      /// The value of the item.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      public override void SetValue (int index, TValue value)
      {
         if (_base != null)
         {
            _base.SetValue(index, value);
         }
         else
         {
            if (index < 0 || index >= _values.Count)
            {
               throw new ArgumentOutOfRangeException("index");
            }

            _values[_keys[index]] = value;
         }
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
      public override TValue this [TKey key]
      {
         get
         {
            TValue value = default(TValue);

            if (_base != null)
            {
               value = _base[key];
            }
            else
            {
               value = _values[key];
            }

            return value;
         }
         set
         {
            if (_base != null)
            {
               _base[key] = value;
            }
            else
            {
					if(_keys.Contains(key))
					{
						_values[key] = value;
					}
					else
					{
						BaseAdd(key, value);
					}
            }
         }
      }

      void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
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
            info.AddValue("Comparer", _values.Comparer);
            bool simpleKey = false;

            switch (Type.GetTypeCode(typeof(TKey)))
            {
               case TypeCode.Boolean:
               case TypeCode.Byte:
               case TypeCode.Char:
               case TypeCode.DateTime:
               case TypeCode.Decimal:
               case TypeCode.Double:
               case TypeCode.Int16:
               case TypeCode.Int32:
               case TypeCode.Int64:
               case TypeCode.SByte:
               case TypeCode.Single:
               case TypeCode.String:
               case TypeCode.UInt16:
               case TypeCode.UInt32:
               case TypeCode.UInt64:
               {
                  simpleKey = true;
                  break;
               }
            }

            info.AddValue("SimpleKey", simpleKey);

            if (simpleKey)
            {
               for (int i = 0; i < count; i++)
               {
                  info.AddValue(KeyAt(i).ToString(), ValueAt(i));
               }
            }
            else
            {
               for (int i = 0; i < count; i++)
               {
                  info.AddValue("K" + i.ToString(), KeyAt(i));
                  info.AddValue("V" + i.ToString(), ValueAt(i));
               }
            }
         }
      }

      void IDictionaryWrapper<TKey, TValue>.InnerAdd (TKey key, TValue value)
      {
         Add(key, value);
      }

      void IDictionaryWrapper<TKey, TValue>.InnerInsert (int index, TKey key, TValue value)
      {
         Insert(index, key, value);
      }
   }
}
