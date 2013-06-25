using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides a class that wraps a collection.
   /// </summary>
   public interface IDictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
   {
      /// <summary>
      /// Adds an item to the inner dictionary.
      /// </summary>
      void InnerAdd (TKey key, TValue value);

      /// <summary>
      /// Adds an item to the inner dictionary.
      /// </summary>
      void InnerInsert (int index, TKey key, TValue value);

      /// <summary>
      /// Gets the key comparer for comparisons.
      /// </summary>
      IEqualityComparer<TKey> Comparer { get; }

      /// <summary>
      /// Returns the key of the specified item, or null if the item does not exist in the collection.
      /// </summary>
      /// <param name="value">
      /// The value whose key is desired.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      TKey KeyOf (TValue value);

      /// <summary>
      /// Gets the key of the item at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item whose key is desired.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      TKey KeyAt (int index);

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
      int GetIndex (TKey key);

      /// <summary>
      /// Returns the keys for the current collection.
      /// </summary>
      TKey[] GetKeys ();

      /// <summary>
      /// Copies the values of the collection to a dictionary.
      /// </summary>
      /// <param name="dictionary">
      /// The destination dictionary which the items will be copied.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if dictionary is null.
      /// </exception>
      void CopyTo (IDictionary dictionary);

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
      bool HasKey (TKey key);

      /// <summary>
      /// Determines if the an item exists in the collection.
      /// </summary>
      /// <param name="value">
      /// The item whose existence is desired.
      /// </param>
      bool Contains (TValue value);

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
      void SetKey (int index, TKey newKey);

      /// <summary>
      /// Returns the value at the specified index.
      /// </summary>
      /// <param name="index">
      /// The index of the item.
      /// </param>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      TValue ValueAt (int index);

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
      TValue ValueOf (TKey key, TValue defaultValue);

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
      void SetValue (int index, TValue value);

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      TValue[] ToArray ();

      /// <summary>
      /// Gets the index of the specified value.
      /// Returns -1 if the value does not exist 
      /// in the collection.
      /// </summary>
      /// <param name="value">
      /// The value whose index is desired.
      /// </param>
      int IndexOf (TValue value);

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
      void RemoveAt (int index);

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
      void Move (int oldIndex, int newIndex);

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
      void Swap (int firstIndex, int secondIndex);

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// fixed-size.
      /// </summary>
      bool IsFixedSize { get; }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// synchronized.
      /// </summary>
      bool IsSynchronized { get; }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// transactional.
      /// </summary>
      bool IsTransactional  { get; }

      /// <summary>
      /// Gets a value that can be used to synchronize access to
      /// the collection.
      /// </summary>
      object SyncRoot { get; }
   }
}
