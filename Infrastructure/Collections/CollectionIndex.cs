using System;
using System.Reflection;
using System.Collections.Generic;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   ///// <summary>
   ///// Provides a class for indexing a collection.
   ///// </summary>
   ///// <typeparam name="TValue">
   ///// The type of the values to be indexed.
   ///// </typeparam>


   /// <summary>
   /// Provides a class for indexing a collection.
   /// </summary>
   /// <typeparam name="TElement">
   /// The type of each element in the indexed collection.
   /// </typeparam>
   /// <typeparam name="TValue">
   /// The type of the values to be indexed.
   /// </typeparam>
   [Serializable]
   public class CollectionIndex<TElement, TValue>
   {
      /// <summary>
      /// The indexes of the properties in the original collection.
      /// </summary>
      private int[] _originalIndexes;

      /// <summary>
      /// Indexed values of the object properties.
      /// </summary>
      private TValue[] _indexedValues;

      private IComparer<TValue> _comparer;

      /// <summary>
      /// Creates collection index based on the specified property.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="property">
      /// The property to index.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, PropertyInfo property) : this(collection, property, Comparer<TValue>.Default) {}

      /// <summary>
      /// Creates collection index based on the specified property.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="property">
      /// The property to index.
      /// </param>
      /// <param name="comparer">
      /// A comparer for comparing property values.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, PropertyInfo property, IComparer<TValue> comparer)
      {
         ValueOfProperty valueOf = new ValueOfProperty(property);
         Compile(collection, new ValueOfDelegate<TElement, TValue>(valueOf.ValueOf), comparer);
      }

      /// <summary>
      /// Creates collection index based on the specified field.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="field">
      /// The field to index.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, FieldInfo field) : this(collection, field, Comparer<TValue>.Default) {}

      /// <summary>
      /// Creates collection index based on the specified field.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="field">
      /// The field to index.
      /// </param>
      /// <param name="comparer">
      /// A comparer for comparing field values.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, FieldInfo field, IComparer<TValue> comparer)
      {
         ValueOfField valueOf = new ValueOfField(field);
         Compile(collection, new ValueOfDelegate<TElement, TValue>(valueOf.ValueOf), comparer);
      }

      /// <summary>
      /// Creates collection index based on the specified property.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="valueOf">
      /// The delegate that retrieves values.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, ValueOfDelegate<TElement, TValue> valueOf) : this(collection, valueOf, Comparer<TValue>.Default) {}

      /// <summary>
      /// Creates collection index based on the specified property.
      /// </summary>
      /// <param name="collection">
      /// The collection of items to index.
      /// </param>
      /// <param name="valueOf">
      /// The delegate that retrieves values.
      /// </param>
      /// <param name="comparer">
      /// A comparer for comparing property values.
      /// </param>
      public CollectionIndex (ICollection<TElement> collection, ValueOfDelegate<TElement, TValue> valueOf, IComparer<TValue> comparer)
      {
         Compile(collection, valueOf, comparer);
      }

      /// <summary>
      /// Returns the zero-based index of the object in the original collection
      /// with the specified property value.
      /// </summary>
      public int[] Search (TValue value)
      {
         int[] found = null;
         DynamicArrayList<int> indexes = new DynamicArrayList<int>();

         // the position in the indexed array.
         int position = -1;

         // find the position in the indexed array in which
         // the value occurs. Note: this may not be the first
         // matching value in the indexed array!
         if (_comparer != null)
         {
            position = Array.BinarySearch<TValue>(_indexedValues, 0, _indexedValues.Length, value, _comparer);
         }
         else
         {
            position = Array.BinarySearch<TValue>(_indexedValues, 0, _indexedValues.Length, value);
         }

         if (position != -1)
         {
            // add the current position to the indexes.
            indexes.Add(_originalIndexes[position]);

            // add all of the matching values before the
            // current matching value.
            for (int i = position - 1; i >= 0; i--)
            {
               if (IsEqual(_indexedValues[i], value))
               {
                  indexes.Add(_originalIndexes[i]);
               }
            }

            // add all of the matching values after the
            // current matching value.
            for (int i = position + 1; i < _indexedValues.Length; i++)
            {
               if (IsEqual(_indexedValues[i], value))
               {
                  indexes.Add(_originalIndexes[i]);
               }
            }

            found = indexes.ToArray();

            if (found.Length != 1)
            {
               Array.Sort<int>(found);
            }
         }

         return found;
      }

      /// <summary>
      /// Returns the zero-based index of the object in the original collection
      /// with the specified property value.
      /// </summary>
      public int IndexOf (TValue value)
      {
         return IndexOf(value, 0);
      }

      /// <summary>
      /// Returns the zero-based index of the object in the original collection
      /// with the specified property value.
      /// </summary>
      public int IndexOf (TValue value, int offset)
      {
         int indexOf = -1;

         // the position in the indexed array.
         int position = -1;

         // find the position in the indexed array in which
         // the value occurs. Note: this may not be the first
         // matching value in the indexed array!
         if (_comparer != null)
         {
            position = Array.BinarySearch<TValue>(_indexedValues, 0, _indexedValues.Length, value, _comparer);
         }
         else
         {
            position = Array.BinarySearch<TValue>(_indexedValues, 0, _indexedValues.Length, value);
         }

         if (position >= 0)
         {
            // find the first matching values.
            for (int i = position - 1; i >= 0; i--)
            {
               if (!IsEqual(_indexedValues[i], value))
               {
                  position = i + 1;
                  break;
               }
            }

            indexOf = _originalIndexes[position];

            // continue until we find the first matching
            // value after the specified offset.
            while (indexOf < offset)
            {
               if (++position < _indexedValues.Length)
               {
                  if (IsEqual(_indexedValues[position], value))
                  {
                     indexOf = _originalIndexes[position];
                  }
                  else
                  {
                     indexOf = -1;
                     break;
                  }
               }
            }
         }

         return indexOf;
      }

      private void Compile (ICollection<TElement> collection, ValueOfDelegate<TElement, TValue> valueOf, IComparer<TValue> comparer)
      {
         if (collection == null)
         {
            throw new ArgumentNullException("Parameter 'collection' cannot be null.");
         }

         if (collection.Count == 0)
         {
            throw new ArgumentException("The specified collection is empty.");
         }

         _comparer = comparer;
         IEnumerator<TElement> enumerator = collection.GetEnumerator();
         DynamicArrayList<int> originalIndexes = new DynamicArrayList<int>();
         DynamicArrayList<TValue> indexedValues = new DynamicArrayList<TValue>();
         int itemIndex = 0;

         while (enumerator.MoveNext())
         {
            TElement currentItem = enumerator.Current;
            originalIndexes.Add(itemIndex++);
            indexedValues.Add(valueOf(currentItem));
         }

         _originalIndexes = originalIndexes.ToArray();
         _indexedValues = indexedValues.ToArray();

         if (comparer != null)
         {
            Array.Sort<TValue, int>(_indexedValues, _originalIndexes, comparer);
         }
         else
         {
            Array.Sort<TValue, int>(_indexedValues, _originalIndexes);
         }
      }

      private bool IsEqual (TValue first, TValue second)
      {
         bool equals = false;

         if (_comparer != null)
         {
            equals = (_comparer.Compare(first, second) == 0);
         }
         else
         {
            equals = first.Equals(second);
         }

         return equals;
      }

      private class ValueOfProperty
      {
         private PropertyInfo _property;

         public ValueOfProperty (PropertyInfo property)
         {
            _property = property;
         }

         public TValue ValueOf (TElement element)
         {
            return (TValue)_property.GetValue(element, null);
         }
      }

      private class ValueOfField
      {
         private FieldInfo _field;

         public ValueOfField (FieldInfo field)
         {
            _field = field;
         }

         public TValue ValueOf (TElement element)
         {
            return (TValue)_field.GetValue(element);
         }
      }
   }
}
