using System;
using System.Reflection;
using System.Collections;
using Infrastructure.Collections;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides helper methods for arrays.
   /// </summary>
   public static class ArrayUtility
   {
      /// <summary>
      /// Returns an array of the specified values.
      /// All of the items in the array must be of
      /// the same type.
      /// </summary>
      /// <param name="values">An array of values.</param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if values is null.
      /// </exception>
      public static Array ToArray (IEnumerable values)
      {
         ArrayList valueList = new ArrayList();
         Type valueType = null;

         foreach (object value in values)
         {
            if (valueType == null)
            {
               valueType = value.GetType();
            }

            valueList.Add(value);
         }

         Array valueArray = null;

         if (valueList.Count != 0)
         {
            valueArray = Array.CreateInstance(valueType, valueList.Count);

            for (int i = 0; i < valueList.Count; i++)
            {
               valueArray.SetValue(valueList[i], i);
            }
         }

         return valueArray;
      }

      /// <summary>
      /// Returns an array of the specified values.
      /// </summary>
      /// <param name="values">An array of values.</param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if values is null.
      /// </exception>
      public static TValue[] ToArray<TValue> (IEnumerable values)
      {
         DynamicArrayList<TValue> valueList = new DynamicArrayList<TValue>();

         foreach (TValue value in values)
         {
            valueList.Add(value);
         }

         return valueList.ToArray();
      }

      /// <summary>
      /// Resizes and returns an array.
      /// </summary>
      /// <param name="array">An array of elements.</param>
      /// <param name="newCapacity">The new capacity of the array.</param>
      /// <returns>A new array of the specified size.</returns>
      /// <exception cref="ArgumentNullException">
      /// Thrown if array or elementType is null.
      /// </exception>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown if index is less than zero.
      /// </exception>
      public static Array Resize (Array array, int newCapacity)
      {
         if (array == null)
         {
            throw new ArgumentNullException("array");
         }

         if (newCapacity < 0)
         {
            throw new ArgumentOutOfRangeException("The parameter \"newCapacity\" must be greater than zero.");
         }

         Type elementType = array.GetType().GetElementType();
         Array temp = Array.CreateInstance(elementType, newCapacity);
         Array.Copy(array, 0, temp, 0, newCapacity);
         return temp;
      }

      /// <summary>
      /// Merges the contents of two arrays.
      /// </summary>
      /// <param name="firstArray">The first array of the merge.</param>
      /// <param name="secondArray">The second array of the merge.</param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if one of the arrays is null.
      /// </exception>
      public static Array Merge (Array firstArray, Array secondArray)
      {
         return Merge(firstArray, secondArray, false);
      }

      /// <summary>
      /// Merges the contents of two arrays.
      /// </summary>
      /// <param name="firstArray">The first array of the merge.</param>
      /// <param name="secondArray">The second array of the merge.</param>
      /// <param name="isSorted">True if the arrays are sorted, otherwise false. 
      /// Also, if this value is true, then the returned array will be sorted.</param>
      /// <exception cref="ArgumentNullException">
      /// Thrown if one of the arrays is null.
      /// </exception>
      public static Array Merge (Array firstArray, Array secondArray, bool isSorted)
      {
         if (firstArray == null)
         {
            throw new ArgumentNullException("firstArray");
         }

         if (secondArray == null)
         {
            throw new ArgumentNullException("secondArray");
         }

         if (!isSorted)
         {
            firstArray = (Array)firstArray.Clone();
            Array.Sort(firstArray);
            secondArray = (Array)secondArray.Clone();
            Array.Sort(secondArray);
         }

         Type elementType = firstArray.GetType().GetElementType();
         int mergedLength = Math.Max(firstArray.Length, secondArray.Length);
         ArrayList mergedList = new ArrayList(mergedLength);

         for (int i = 0; i < firstArray.Length; i++)
         {
            object firstValue = firstArray.GetValue(i);
            mergedList.Add(firstValue);
         }

         for (int i = 0; i < secondArray.Length; i++)
         {
            object secondValue = secondArray.GetValue(i);

            if (!mergedList.Contains(secondValue))
            {
               mergedList.Add(secondValue);
            }
         }

         Array mergedArray = Array.CreateInstance(elementType, mergedList.Count);
         mergedList.CopyTo(mergedArray, 0);

         if (isSorted)
         {
            Array.Sort(mergedArray);
         }

         return mergedArray;
      }

      /// <summary>
      /// Sorts the values in the collection by the specified
      /// property, field, or method.
      /// </summary>
      public static Array SortBy (Array array, string propertyOrMethod)
      {
         Type valueType = array.GetType().GetElementType();
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

         object[] sortBy = new object[array.Length];
         Array values = Array.CreateInstance(valueType, array.Length);

         if (propertyInfo != null)
         {
            for (int i = 0; i < array.Length; i++)
            {
               object value = array.GetValue(i);
               sortBy[i] = propertyInfo.GetValue(value, null);
               values.SetValue(value, i);
            }
         }
         else if (fieldInfo != null)
         {
            for (int i = 0; i < array.Length; i++)
            {
               object value = array.GetValue(i);
               sortBy[i] = fieldInfo.GetValue(value);
               values.SetValue(value, i);
            }
         }
         else if (methodInfo != null)
         {
            for (int i = 0; i < array.Length; i++)
            {
               object value = array.GetValue(i);
               sortBy[i] = methodInfo.Invoke(value, null);
               values.SetValue(value, i);
            }
         }

         Array.Sort(sortBy, values);
         return values;
      }

      /// <summary>
      /// Sorts the values in the collection by the specified
      /// property, field, or method.
      /// </summary>
      public static T[] SortBy<T> (T[] array, string propertyOrMethod)
      {
         return (T[])SortBy((Array)array, propertyOrMethod);
      }
   }
}
