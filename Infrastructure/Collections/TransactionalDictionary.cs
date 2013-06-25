using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.Collections;


namespace Infrastructure.Collections
{
   /// <summary>
   /// Represents a transacted collection.
   /// </summary>
   public sealed class TransactionalDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>, IDictionaryWrapper<TKey, TValue>
   {
      private TransactionWrapper<DictionaryBase<TKey, TValue>> _wrapper;

      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      public TransactionalDictionary (DictionaryBase<TKey, TValue> values)
      {
         _wrapper = new TransactionWrapper<DictionaryBase<TKey, TValue>>(values);
      }

      /// <summary>
      /// Gets the number of items in the collection.
      /// </summary>
      public override int Count
      {
         get
         {
            return _wrapper.Value.Count;
         }
      }

      /// <summary>
      /// Gets the key comparer for comparisons.
      /// </summary>
      public override IEqualityComparer<TKey> Comparer
      {
         get
         {
            return _wrapper.Value.Comparer;
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
            return _wrapper.Value.IsReadOnly;
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
            return _wrapper.Value.IsFixedSize;
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
            return _wrapper.Value.IsSynchronized;
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
            return true;
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
            return _wrapper.Value.SyncRoot;
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
         return _wrapper.Value.KeyOf(value);
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
         return _wrapper.Value.KeyAt(index);
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
         return _wrapper.Value.IndexOf(value);
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
         return _wrapper.Value.GetIndex(key);
      }

      /// <summary>
      /// Returns the keys for the current collection.
      /// </summary>
      public override TKey[] GetKeys ()
      {
         return _wrapper.Value.GetKeys();
      }

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      public override TValue[] ToArray ()
      {
         return _wrapper.Value.ToArray();
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
         _wrapper.Value.BaseAdd(key, value);
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
         _wrapper.Value.BaseInsert(index, key, value);
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
         _wrapper.Value.Remove(key);
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
         _wrapper.Value.RemoveAt(index);
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
         _wrapper.Value.Clear();
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
         _wrapper.Value.Move(oldIndex, newIndex);
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
         _wrapper.Value.Swap(firstIndex, secondIndex);
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
         return _wrapper.Value.HasKey(key);
      }

      /// <summary>
      /// Determines if the an item exists in the collection.
      /// </summary>
      /// <param name="value">
      /// The item whose existence is desired.
      /// </param>
      public override bool Contains (TValue value)
      {
         return _wrapper.Value.Contains(value);
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
      public override void SetKey (int index, TKey newKey)
      {
         _wrapper.Value.SetKey(index, newKey);
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
         return _wrapper.Value.ValueAt(index);
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
         _wrapper.Value.SetValue(index, value);
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
            return _wrapper.Value[key];
         }

         set
         {
            _wrapper.Value[key] = value;
         }
      }

      /// <summary>
      /// Returns an enumerator for iterating over the collection.
      /// </summary>
      public override IEnumerator GetEnumerator ()
      {
         return _wrapper.Value.GetEnumerator();
      }

      void IDictionaryWrapper<TKey, TValue>.InnerAdd (TKey key, TValue value)
      {
         BaseAdd(key, value);
      }

      void IDictionaryWrapper<TKey, TValue>.InnerInsert (int index, TKey key, TValue value)
      {
         BaseInsert(index, key, value);
      }
   }
}
