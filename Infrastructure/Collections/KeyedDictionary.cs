using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides the base class for keyed dictionaries (as opposed to 
   /// custom dictionaries).
   /// </summary>
   [Serializable]
   public abstract class KeyedDictionary<TKey, TValue> : DictionaryBase<TKey, TValue>
   {
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
      public void Add (TKey key, TValue value)
      {
         BaseAdd(key, value);
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
      public void Insert (int index, TKey key, TValue value)
      {
         BaseInsert(index, key, value);
      }

      /// <summary>
      /// Returns an enumerator for iterating over the keys and
      /// values in the collection.
      /// </summary>
      public override IEnumerator GetEnumerator ()
      {
         return new KeyValueEnumerator<TKey, TValue>(GetKeys(), ToArray());
      }
   }
}
