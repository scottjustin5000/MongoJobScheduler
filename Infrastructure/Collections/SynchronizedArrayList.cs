using System;
using System.Collections.Generic;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Represents a synchronized collection.
   /// </summary>
   public sealed class SynchronizedArrayList<TValue> : ArrayListBase<TValue>
   {
      private ArrayListBase<TValue> _values;
      private object _syncRoot = new object();

      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      public SynchronizedArrayList (ArrayListBase<TValue> values)
      {
         _values = values;
      }

      /// <summary>
      /// Gets the number of items in the collection.
      /// </summary>
      public override int Count
      {
         get
         {
            int count = 0;

            lock (_syncRoot)
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
            return _values.IsReadOnly;
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
            return _values.IsFixedSize;
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
            return true;
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
            return _values.IsTransactional;
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
            return _syncRoot;
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
      public override int IndexOf (TValue value)
      {
         int index = -1;

         lock (_syncRoot)
         {
            index = _values.IndexOf(value);
         }

         return index;
      }

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      public override TValue[] ToArray ()
      {
         TValue[] values = null;

         lock (_syncRoot)
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
      public override void Add (TValue value)
      {
         lock (_syncRoot)
         {
            _values.Add(value);
         }
      }

      /// <summary>
      /// Adds an array of items to the collection.
      /// </summary>
      /// <param name="values">
      /// The array of items.
      /// </param>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      public override void Add (TValue[] values)
      {
         lock (_syncRoot)
         {
            _values.Add(values);
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
      public override void Insert (int index, TValue value)
      {
         lock (_syncRoot)
         {
            _values.Insert(index, value);
         }
      }

      /// <summary>
      /// Removes an item from the collection.
      /// </summary>
      /// <param name="value">
      /// The item to remove.
      /// </param>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      public override void Remove (TValue value)
      {
         lock (_syncRoot)
         {
            _values.Remove(value);
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
         lock (_syncRoot)
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
      public override void Clear ()
      {
         lock (_syncRoot)
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
      public override void Move (int oldIndex, int newIndex)
      {
         lock (_syncRoot)
         {
            _values.Move(oldIndex, newIndex);
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
         lock (_syncRoot)
         {
            _values.Swap(firstIndex, secondIndex);
         }
      }

      /// <summary>
      /// Determines if the an item exists in the collection.
      /// </summary>
      /// <param name="value">
      /// The item whose existence is desired.
      /// </param>
      public override bool Contains (TValue value)
      {
         bool contains = false;

         lock (_syncRoot)
         {
            contains = _values.Contains(value);
         }

         return contains;
      }

      /// <summary>
      /// Gets/sets an item in the collection.
      /// </summary>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if the index is out-of-range.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the set operation is called and the collection is read-only.
      /// </exception>
      public override TValue this [int index]
      {
         get
         {
            TValue value = default(TValue);

            lock (_syncRoot)
            {
               value = _values[index];
            }

            return value;
         }

         set
         {
            lock (_syncRoot)
            {
               _values[index] = value;
            }
         }
      }
   }
}
