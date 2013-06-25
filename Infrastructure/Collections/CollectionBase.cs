using System;
using System.Reflection;
using System.Collections;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides the base class for a dictionary of objects that preserves 
   /// the order in which items are added to the collection.
   /// </summary>
   [Serializable]
   public abstract class CollectionBase<TValue> : IEnumerable
   {
      private DynamicDictionary<string, CollectionIndex<TValue, object>> _indexes;
      private DynamicDictionary<string[], string> _keysByProperties;

      /// <summary>
      /// Creates a new instance of this type.
      /// </summary>
      protected CollectionBase () {}

      /// <summary>
      /// Gets the number of items in the collection.
      /// </summary>
      public abstract int Count
      {
         get;
      }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// read-only.
      /// </summary>
      public virtual bool IsReadOnly
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// fixed-size.
      /// </summary>
      public virtual bool IsFixedSize
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// synchronized.
      /// </summary>
      public virtual bool IsSynchronized
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Gets a value that specifies if the collection is 
      /// transactional.
      /// </summary>
      public virtual bool IsTransactional
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Gets a value that can be used to synchronize access to
      /// the collection.
      /// </summary>
      public virtual object SyncRoot
      {
         get
         {
            return null;
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
      public abstract int IndexOf (TValue value);

      /// <summary>
      /// Determines if the an item exists in the collection.
      /// </summary>
      /// <param name="value">
      /// The item whose existence is desired.
      /// </param>
      public abstract bool Contains (TValue value);

      /// <summary>
      /// Indexes a property of the specified property
      /// of the contained value type. The indexes created
      /// by this method are used in the Search methods.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if property is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if property is empty.
      /// </exception>
      public void Compile (string property)
      {
         if (property == null)
         {
            throw new ArgumentNullException("property");
         }

         if (property.Length == 0)
         {
            throw new ArgumentException("The parameter \"property\" must be specified.");
         }

         if (_indexes == null)
         {
            _indexes = new DynamicDictionary<string, CollectionIndex<TValue, object>>();
         }

         if (_indexes.HasKey(property))
         {
            _indexes.Remove(property);
         }

         if (Count != 0)
         {
            Type type = typeof(TValue);
            CollectionIndex<TValue, object> index = new CollectionIndex<TValue, object>(ToArray(), type.GetProperty(property));
            _indexes.Add(property, index);
         }
      }

      /// <summary>
      /// Creates and compiles an index on multiple 
      /// properties of the contained value type. 
      /// The indexes created by this method are 
      /// used in the Search methods.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if indexName or properties is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if indexName or properties is empty.
      /// </exception>
      public void Compile (string[] properties)
      {
         if (properties == null)
         {
            throw new ArgumentNullException("properties");
         }

         string key = "";

         for (int i = 0; i < properties.Length; i++)
         {
            key += properties[i] + "|";
         }

         if (key.Length == 0)
         {
            throw new ArgumentException("One or more properties must be specified.");
         }

         key = key.Substring(0, key.Length - 1);

         if (_indexes == null)
         {
            _indexes = new DynamicDictionary<string, CollectionIndex<TValue, object>>();
         }

         if (_indexes.HasKey(key))
         {
            _indexes.Remove(key);
         }

         if (Count != 0)
         {
            ValueOfProperties valueOfProperties = new ValueOfProperties(properties);
            CollectionIndex<TValue, object> index = new CollectionIndex<TValue, object>(ToArray(), valueOfProperties.ValueOf);
            _indexes.Add(key, index);

            if (_keysByProperties == null)
            {
               _keysByProperties = new DynamicDictionary<string[], string>();
            }

            _keysByProperties.Add(properties, key);
         }
      }

      /// <summary>
      /// Merges the current collection with the specified
      /// collection using the specified set operation.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if values is null.
      /// </exception>
      public TValue[] Merge (ArrayListBase<TValue> values, MergeOperation operation)
      {
         if (values == null)
         {
            throw new ArgumentNullException("values");
         }

         return Merge(values.ToArray(), operation);
      }

      /// <summary>
      /// Merges the current collection with the specified
      /// array using the specified set operation.
      /// </summary>
      /// <exception cref="ArgumentNullException">
      /// Thrown if values is null.
      /// </exception>
      public TValue[] Merge (TValue[] values, MergeOperation operation)
      {
         if (values == null)
         {
            throw new ArgumentNullException("values");
         }

         DynamicArrayList<TValue> set = new DynamicArrayList<TValue>();

         switch (operation)
         {
            case MergeOperation.Intersection:
            {
               for (int i = 0; i < values.Length; i++)
               {
                  TValue value = values[i];

                  if (Contains(value))
                  {
                     set.Add(value);
                  }
               }

               break;
            }

            case MergeOperation.Union:
            {
               TValue[] array = ToArray();

               for (int i = 0; i < array.Length; i++)
               {
                  TValue value = array[i];

                  if (!set.Contains(value))
                  {
                     set.Add(value);
                  }
               }

               for (int i = 0; i < values.Length; i++)
               {
                  TValue value = values[i];

                  if (!set.Contains(value))
                  {
                     set.Add(value);
                  }
               }

               break;
            }

            case MergeOperation.Difference:
            {
               TValue[] array = ToArray();
               DynamicArrayList<TValue> arrayList = new DynamicArrayList<TValue>(values.Length);
               arrayList.Add(values);

               for (int i = 0; i < values.Length; i++)
               {
                  TValue value = array[i];

                  if (!arrayList.Contains(value))
                  {
                     set.Add(value);
                  }
               }

               break;
            }
         }

         return set.ToArray();
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="property">
      /// The property of the element to search.
      /// </param>
      /// <param name="value">
      /// The value for which to search.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public int Search (string property, object value)
      {
         return Search(property, value, 0, null);
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="property">
      /// The property of the element to search.
      /// </param>
      /// <param name="value">
      /// The value for which to search.
      /// </param>
      /// <param name="start">
      /// The index at which searching should begin.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public int Search (string property, object value, int start)
      {
         return Search(property, value, start, null);
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="property">
      /// The property of the element to search.
      /// </param>
      /// <param name="value">
      /// The value for which to search.
      /// </param>
      /// <param name="start">
      /// The index at which searching should begin.
      /// </param>
      /// <param name="comparer">
      /// The comparer to use while searching.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public virtual int Search (string property, object value, int start, IEqualityComparer comparer)
      {
         if (property == null)
         {
            throw new ArgumentNullException("property");
         }

         if (property.Length == 0)
         {
            throw new ArgumentException("Parameter \"property\" must be specified.");
         }

         int found = -1;

         if (start < Count)
         {
            if (_indexes != null && _indexes.HasKey(property))
            {
               CollectionIndex<TValue, object> index = _indexes[property];
               found = index.IndexOf(value, start);
            }
            else
            {
               TValue[] values = ToArray();
               DynamicDictionary<Type, object> propertiesOrFields = new DynamicDictionary<Type, object>();

               for (int i = start; i < values.Length; i++)
               {
                  TValue compareItem = values[i];

                  if (compareItem != null)
                  {
                     Type compareType = compareItem.GetType();
                     object propertyOrField = null;

                     if (propertiesOrFields.HasKey(compareType))
                     {
                        propertyOrField = propertiesOrFields[compareType];
                     }
                     else
                     {
                        propertyOrField = compareType.GetProperty(property);

                        if (propertyOrField == null)
                        {
                           propertyOrField = compareType.GetField(property);
                        }

                        propertiesOrFields.Add(compareType, propertyOrField);
                     }

                     object compareValue = null;

                     if (propertyOrField != null)
                     {
                        if (propertyOrField is PropertyInfo)
                        {
                           compareValue = ((PropertyInfo)propertyOrField).GetValue(compareItem, null);
                        }
                        else
                        {
                           compareValue = ((FieldInfo)propertyOrField).GetValue(compareItem);
                        }

                        if (compareValue != null)
                        {
                           if (value != null)
                           {
                              if (comparer != null)
                              {
                                 if (comparer.Equals(value, compareValue))
                                 {
                                    found = i;
                                 }
                              }
                              else if (compareValue.Equals(value))
                              {
                                 found = i;
                              }
                           }
                        }
                        else if (value == null)
                        {
                           found = i;
                        }

                        if (found != -1)
                        {
                           break;
                        }
                     }
                  }
               }
            }
         }

         return found;
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="properties">
      /// The properties of the element to search.
      /// </param>
      /// <param name="values">
      /// The values for which to search.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public int Search (string[] properties, object[] values)
      {
         return Search(properties, values, 0, null);
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="properties">
      /// The properties of the element to search.
      /// </param>
      /// <param name="values">
      /// The values for which to search.
      /// </param>
      /// <param name="start">
      /// The index at which searching should begin.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public int Search (string[] properties, object[] values, int start)
      {
         return Search(properties, values, start, null);
      }

      /// <summary>
      /// Finds the first object in the collection
      /// that contains the specified value for the
      /// indicated property. Returns the index of 
      /// the found item, or -1 if an item with the 
      /// specified property value cannot be found 
      /// in the collection.
      /// </summary>
      /// <param name="properties">
      /// The properties of the element to search.
      /// </param>
      /// <param name="values">
      /// The values for which to search.
      /// </param>
      /// <param name="start">
      /// The index at which searching should begin.
      /// </param>
      /// <param name="comparer">
      /// The comparer to use while searching.
      /// </param>
      /// <returns>
      /// The index of the found item, or -1 if an
      /// item with the specified property value 
      /// cannot be found in the collection.
      /// </returns>
      public virtual int Search (string[] properties, object[] values, int start, IEqualityComparer comparer)
      {
         if (properties == null)
         {
            throw new ArgumentNullException("properties");
         }

         if (properties.Length == 0)
         {
            throw new ArgumentException("Parameter \"properties\" must be specified.");
         }

         int found = -1;

         if (start < Count)
         {
            CollectionIndex<TValue, object> index = null;

            if (_indexes != null && _keysByProperties != null)
            {
               string key = null;

               if (_keysByProperties.HasKey(properties))
               {
                  key = _keysByProperties[properties];
               }
               else
               {
                  int foundIndex = -1;

                  foreach (string[] exitingProperties in _keysByProperties.GetKeys())
                  {
                     if (exitingProperties.Length == properties.Length)
                     {
                        bool isEqual = true;

                        for (int i = 0; i < properties.Length; i++)
                        {
                           if (exitingProperties[i] != properties[i])
                           {
                              isEqual = false;
                              break;
                           }
                        }

                        if (isEqual)
                        {
                           foundIndex = _keysByProperties.GetIndex(exitingProperties);
                           break;
                        }
                     }
                  }

                  if (foundIndex != -1)
                  {
                     _keysByProperties.SetKey(foundIndex, properties);
                  }
                  else
                  {
                     key = "";

                     for (int i = 0; i < properties.Length; i++)
                     {
                        key += properties[i] + "|";
                     }

                     key = key.Substring(0, key.Length - 1);
                     _keysByProperties.Add(properties, key);
                  }
               }

               index = _indexes[key];
            }

            if (index != null)
            {
               string buffer = "";

               for (int i = 0; i < values.Length; i++)
               {
                  object value = values[i];

                  if (value != null)
                  {
                     buffer += value.ToString() + "|";
                  }
                  else
                  {
                     buffer += "|";
                  }
               }

               buffer = buffer.Substring(0, buffer.Length - 1);
               found = index.IndexOf(buffer, start);
            }
            else
            {
               //TValue[] values = ToArray();
               //DynamicDictionary<Type, object> propertiesOrFields = new DynamicDictionary<Type, object>();

               //for (int i = start; i < values.Length; i++)
               //{
               //   TValue compareItem = values[i];

               //   if (compareItem != null)
               //   {
               //      Type compareType = compareItem.GetType();
               //      object propertyOrField = null;

               //      if (propertiesOrFields.HasKey(compareType))
               //      {
               //         propertyOrField = propertiesOrFields[compareType];
               //      }
               //      else
               //      {
               //         propertyOrField = compareType.GetProperty(property);

               //         if (propertyOrField == null)
               //         {
               //            propertyOrField = compareType.GetField(property);
               //         }

               //         propertiesOrFields.Add(compareType, propertyOrField);
               //      }

               //      object compareValue = null;

               //      if (propertyOrField != null)
               //      {
               //         if (propertyOrField is PropertyInfo)
               //         {
               //            compareValue = ((PropertyInfo)propertyOrField).GetValue(compareItem, null);
               //         }
               //         else
               //         {
               //            compareValue = ((FieldInfo)propertyOrField).GetValue(compareItem);
               //         }

               //         if (compareValue != null)
               //         {
               //            if (value != null)
               //            {
               //               if (comparer != null)
               //               {
               //                  if (comparer.Equals(value, compareValue))
               //                  {
               //                     found = i;
               //                  }
               //               }
               //               else if (compareValue.Equals(value))
               //               {
               //                  found = i;
               //               }
               //            }
               //         }
               //         else if (value == null)
               //         {
               //            found = i;
               //         }

               //         if (found != -1)
               //         {
               //            break;
               //         }
               //      }
               //   }
               //}
            }
         }

         return found;
      }

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      public abstract TValue[] ToArray ();

      /// <summary>
      /// Returns the values of the current collection in an 
      /// array. The values are in the same order in which they 
      /// exist in the collection.
      /// </summary>
      /// <param name="elementType">
      /// The element type of the array.
      /// </param>
      /// <exception cref="InvalidCastException">
      /// Thrown if the element type of the collection cannot
      /// be cast to the specified element type.
      /// </exception>
      public Array ToArray (Type elementType)
      {
         TValue[] values = ToArray();
         Array array = Array.CreateInstance(elementType, values.Length);

         for (int i = 0; i < values.Length; i++)
         {
            array.SetValue(values[i], i);
         }

         return array;
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
      public abstract void RemoveAt (int index);

      /// <summary>
      /// Clears the collection of all items.
      /// </summary>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is read-only.
      /// </exception>
      /// <exception cref="NotSupportedException">
      /// Thrown if the collection is fixed-size.
      /// </exception>
      public abstract void Clear ();

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
      public abstract void Move (int oldIndex, int newIndex);

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
      public abstract void Swap (int firstIndex, int secondIndex);

      /// <summary>
      /// Copies the values of the collection to an array.
      /// </summary>
      /// <param name="array">
      /// The destination array which the items will be copied.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if array is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the number of elements in the source collection 
      /// is greater than the number of elements in the destination 
      /// array. 
      /// </exception>
      public void CopyTo (Array array)
      {
         CopyTo(array, 0);
      }

      /// <summary>
      /// Copies the values of the collection to an array.
      /// </summary>
      /// <param name="array">
      /// The destination array which the items will be copied.
      /// </param>
      /// <param name="index">
      /// The index of the destination array at which copying should begin.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if array is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the number of elements in the source collection 
      /// is greater than the available space from index to the end 
      /// of the destination array. 
      /// </exception>
      public void CopyTo (Array array, int index)
      {
         if (array == null)
         {
            throw new ArgumentNullException("array");
         }

         if (array.Length - index < Count)
         {
            throw new ArgumentException("The number of elements in the source collection is greater than the available space from index to the end of the destination array.");
         }

         TValue[] values = ToArray();

         for (int i = 0; i < Count; i++)
         {
            array.SetValue(values[i], index + i);
         }
      }
      
      /// <summary>
      /// Copies the values of the collection to an array.
      /// </summary>
      /// <param name="array">
      /// The destination array which the items will be copied.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if array is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the number of elements in the source collection 
      /// is greater than the number of elements in the destination 
      /// array. 
      /// </exception>
      public void CopyTo (TValue[] array)
      {
         CopyTo(array, 0);
      }

      /// <summary>
      /// Copies the values of the collection to an array.
      /// </summary>
      /// <param name="array">
      /// The destination array which the items will be copied.
      /// </param>
      /// <param name="index">
      /// The index of the destination array at which copying should begin.
      /// </param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if array is null.
      /// </exception>
      /// <exception cref="ArgumentException">
      /// Thrown if the number of elements in the source collection 
      /// is greater than the available space from index to the end 
      /// of the destination array. 
      /// </exception>
      public void CopyTo (TValue[] array, int index)
      {
         if (array == null)
         {
            throw new ArgumentNullException("array");
         }

         if (array.Length - index < Count)
         {
            throw new ArgumentException("The number of elements in the source collection is greater than the available space from index to the end of the destination array.");
         }

         TValue[] values = ToArray();

         for (int i = 0; i < Count; i++)
         {
            array[index + i] = values[i];
         }
      }

      /// <summary>
      /// Sorts the values in the collection by the specified
      /// property or method.
      /// </summary>
      public TValue[] SortBy (string propertyOrMethod)
      {
         Type valueType = typeof(TValue);
         PropertyInfo propertyInfo = valueType.GetProperty(propertyOrMethod, BindingFlags.Public | BindingFlags.Instance);
         FieldInfo fieldInfo = null;
         MethodInfo methodInfo = null;

         if (propertyInfo == null)
         {
            fieldInfo = valueType.GetField(propertyOrMethod, BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo == null)
            {
               methodInfo = valueType.GetMethod(propertyOrMethod, BindingFlags.Public | BindingFlags.Instance);

               if (methodInfo == null)
               {
                  throw new ArgumentException("A property, field, or method named \"" + propertyOrMethod + "\" could not be found for type \"" + valueType.FullName + "\".");
               }
            }
         }

         TValue[] values = ToArray();
         object[] sortBy = new object[values.Length];

         if (propertyInfo != null)
         {
            for (int i = 0; i < values.Length; i++)
            {
               sortBy[i] = propertyInfo.GetValue(values[i], null);
            }
         }
         else if (fieldInfo != null)
         {
            for (int i = 0; i < values.Length; i++)
            {
               sortBy[i] = fieldInfo.GetValue(values[i]);
            }
         }
         else if (methodInfo != null)
         {
            for (int i = 0; i < values.Length; i++)
            {
               sortBy[i] = methodInfo.Invoke(values[i], null);
            }
         }

         Array.Sort(sortBy, values);
         return values;
      }

      /// <summary>
      /// Returns an enumerator for iterating over the collection.
      /// </summary>
      public abstract IEnumerator GetEnumerator ();

      private class ValueOfProperties
      {
         private PropertyInfo[] _properties;

         public ValueOfProperties (string[] properties)
         {
            Type valueType = typeof(TValue);
            _properties = new PropertyInfo[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
               PropertyInfo property = valueType.GetProperty(properties[i]);
               _properties[i] = property;
            }
         }

         public object ValueOf (TValue element)
         {
            string buffer = "";

            for (int i = 0; i < _properties.Length; i++)
            {
               PropertyInfo property = _properties[i];
               object value = property.GetValue(element, null);

               if (value != null)
               {
                  buffer += value.ToString() + "|";
               }
               else
               {
                  buffer += "|";
               }
            }

            buffer = buffer.Substring(0, buffer.Length - 1);
            return buffer;
         }
      }
   }
}
