using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides the base class for a dictionary of objects that preserves 
   /// the order in which items are added to the collection.
   /// </summary>
   [Serializable]
   public abstract class DictionaryBase<TKey, TValue> : CollectionBase<TValue>, IDictionary<TKey, TValue>, IDictionary, IEnumerable<KeyValuePair<TKey, TValue>>
   {
      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      protected DictionaryBase () {}

      /// <summary>
      /// Gets the key comparer for comparisons.
      /// </summary>
      public abstract IEqualityComparer<TKey> Comparer
      {
         get;
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
      public abstract TKey KeyOf (TValue value);

      /// <summary>
      /// Gets the key of the item at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item whose key is desired.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      public abstract TKey KeyAt (int index);

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
      public abstract int GetIndex (TKey key);

      /// <summary>
      /// Returns the keys for the current collection.
      /// </summary>
      public abstract TKey[] GetKeys ();

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
      protected internal abstract void BaseAdd (TKey key, TValue value);

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
      protected internal abstract void BaseInsert (int index, TKey key, TValue value);

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
      /// Thrown if an item does not exist in the collection with 
      /// the specified key.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      public abstract void Remove (TKey key);

      /// <summary>
      /// Copies the values of the collection to an array.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if dictionary is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the number of elements in the source collection 
      /// is greater than the available space from index to the end 
      /// of the destination array. 
      /// </exception>
      public void CopyTo (KeyValuePair<TKey, TValue>[] array, int index)
      {
         if (array == null)
         {
            throw new ArgumentNullException("array");
         }

         if (array.Length - index < Count)
         {
            throw new ArgumentException("The number of elements in the source collection is greater than the available space from index to the end of the destination array.");
         }

         for (int i = 0; i < Count; i++)
         {
            array[i] = new KeyValuePair<TKey, TValue>(KeyAt(i), ValueAt(i));
         }
      }

      /// <summary>
      /// Copies the values of the collection to a dictionary.
      /// </summary>
      /// <param name="dictionary">
      /// The destination dictionary which the items will be copied.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if dictionary is null.
      /// </exception>
      public void CopyTo (IDictionary dictionary)
      {
         if (dictionary == null)
         {
            throw new ArgumentNullException("dictionary");
         }

         for (int i = 0; i < Count; i++)
         {
            TKey key = KeyAt(i);
            dictionary.Add(key, ValueAt(i));
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
      public abstract bool HasKey (TKey key);

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
      /// Thrown if the old key or the new key is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the an item with the old key does not exist 
      /// in the collection or an item already exists in the 
      /// collection with the specified new key.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      public abstract void SetKey (int index, TKey newKey);

      /// <summary>
      /// Gets/sets an item in the collection.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the set operation is called and the collection is read-only.
      /// </exception>
      public abstract TValue this[TKey key]
      {
         get;
         set;
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
      public abstract TValue ValueAt (int index);

      /// <summary>
      /// Returns the value of the specified key, or the 
      /// default value if the key is not in the collection.
      /// </summary>
      /// <param name="key">
      /// The key of the item.
      /// </param>
      /// <param name="defaultValue">
      /// The default value of the item.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      public TValue ValueOf (TKey key, TValue defaultValue)
      {
         TValue value = default(TValue);

         if (HasKey(key))
         {
            value = this[key];
         }
         else
         {
            value = defaultValue;
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
      public abstract void SetValue (int index, TValue value);

      /// <summary>
      /// Returns an enumerator for iterating over the keys 
      /// in the collection.
      /// </summary>
      IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
      {
         return new KeyValueEnumerator<TKey, TValue>(GetKeys(), ToArray());
      }

      ICollection IDictionary.Keys
      {
         get
         {
            return GetKeys();
         }
      }

      ICollection IDictionary.Values
      {
         get
         {
            return ToArray();
         }
      }

      bool IDictionary.Contains (object key)
      {
         return HasKey((TKey)key);
      }

      void IDictionary.Add (object key, object value)
      {
         BaseAdd((TKey)key, (TValue)value);
      }

      void IDictionary.Remove (object key)
      {
         Remove((TKey)key);
      }

      object IDictionary.this[object key]
      {
         get
         {
            return this[(TKey)key];
         }

         set
         {
            this[(TKey)key] = (TValue)value;
            ;
         }
      }

      IDictionaryEnumerator IDictionary.GetEnumerator ()
      {
         return new DictionaryEnumerator(this);
      }

      ICollection<TKey> IDictionary<TKey, TValue>.Keys
      {
         get
         {
            return GetKeys();
         }
      }

      ICollection<TValue> IDictionary<TKey, TValue>.Values
      {
         get
         {
            return ToArray();
         }
      }

      bool IDictionary<TKey, TValue>.ContainsKey (TKey key)
      {
         return HasKey(key);
      }

      void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
      {
         BaseAdd(key, value);
      }

      bool IDictionary<TKey, TValue>.Remove (TKey key)
      {
         bool success = HasKey(key);

         if (success)
         {
            Remove(key);
         }

         return success;
      }

      bool IDictionary<TKey, TValue>.TryGetValue (TKey key, out TValue value)
      {
         value = default(TValue);
         bool success = HasKey(key);

         if (success)
         {
            value = this[key];
         }

         return success;
      }

      void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> keyValuePair)
      {
         BaseAdd(keyValuePair.Key, keyValuePair.Value);
      }

      bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> keyValuePair)
      {
         return HasKey(keyValuePair.Key);
      }

      bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> keyValuePair)
      {
         bool success = HasKey(keyValuePair.Key);

         if (success)
         {
            Remove(keyValuePair.Key);
         }

         return success;
      }

      private class DictionaryEnumerator : IDictionaryEnumerator
      {
         private DictionaryBase<TKey, TValue> _collection;
         private int _currentIndex;

         public DictionaryEnumerator (DictionaryBase<TKey, TValue> collection)
         {
            _collection = collection;
            _currentIndex = -1;
         }

         public void Reset ()
         {
            _currentIndex = -1;
         }

         public bool MoveNext ()
         {
            return (++_currentIndex < _collection.Count);
         }

         public object Current
         {
            get
            {
               return Entry;
            }
         }

         public object Key
         {
            get
            {
               return _collection.KeyAt(_currentIndex);
            }
         }

         public object Value
         {
            get
            {
               return _collection.ValueAt(_currentIndex);
            }
         }

         public DictionaryEntry Entry
         {
            get
            {
               return new DictionaryEntry(_collection.KeyAt(_currentIndex), _collection.ValueAt(_currentIndex));
            }
         }
      }
   }
}
