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
   public abstract class ArrayListBase<TValue> : CollectionBase<TValue>, IList<TValue>, IList
   {
      private IEqualityComparer<TValue> _comparer;

      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      protected ArrayListBase () {}

      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      protected ArrayListBase (IEqualityComparer<TValue> comparer)
      {
         _comparer = comparer;
      }

      /// <summary>
      /// Gets the value comparer for comparisons.
      /// </summary>
      public virtual IEqualityComparer<TValue> Comparer
      {
         get
         {
            return _comparer;
         }
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
      public abstract void Add (TValue value);

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
      public abstract void Add (TValue[] values);

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
      public abstract void Insert (int index, TValue value);

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
      public abstract void Remove (TValue value);

      /// <summary>
      /// Gets/sets an item in the collection.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if the key is null.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the set operation is called and the collection is read-only.
      /// </exception>
      public abstract TValue this [int index]
      {
         get;
         set;
      }

      /// <summary>
      /// Returns an enumerator for iterating over the 
      /// values of the collection.
      /// </summary>
      public override IEnumerator GetEnumerator ()
      {
         return new ValueEnumerator(ToArray());
      }

      IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator ()
      {
         return (IEnumerator<TValue>)GetEnumerator();
      }

      bool ICollection<TValue>.Remove (TValue value)
      {
         bool success = false;

         if (Contains(value))
         {
            Remove(value);
            success = true;
         }

         return success;
      }

      int IList.Add (object value)
      {
         Add((TValue)value);
         return Count - 1;
      }

      void IList.Insert (int index, object value)
      {
         Insert(index, (TValue)value);
      }

      void IList.Remove (object value)
      {
         Remove((TValue)value);
      }

      bool IList.Contains (object value)
      {
         return Contains((TValue)value);
      }

      int IList.IndexOf (object value)
      {
         return IndexOf((TValue)value);
      }

      object IList.this[int index]
      {
         get
         {
            return this[index];
         }

         set
         {
            this[index] = (TValue)value;
         }
      }

      private class ValueEnumerator : IEnumerator<TValue>
      {
         private TValue[] _values;
         private int _currentIndex;

         public ValueEnumerator (TValue[] values)
         {
            _values = values;
            _currentIndex = -1;
         }

         public void Reset ()
         {
            _currentIndex = -1;
         }

         public TValue Current
         {
            get
            {
               return _values[_currentIndex];
            }
         }

         object IEnumerator.Current
         {
            get
            {
               return Current;
            }
         }

         public bool MoveNext ()
         {
            return (++_currentIndex < _values.Length);
         }

         void IDisposable.Dispose ()
         {
            _values = null;
            _currentIndex = -1;
         }
      }
   }
}
