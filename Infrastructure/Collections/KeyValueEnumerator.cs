using System;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides a class for enumerating over key/value pairs.
   /// </summary>
   public class KeyValueEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
   {
      private TKey[] _keys;
      private TValue[] _values;
      private ValueOfDelegate<TValue> _valueOf;
      private int _index;

      /// <summary>
      /// Creates a new instance of type KeyValueEnumerator.
      /// </summary>
      public KeyValueEnumerator (TKey[] keys, TValue[] values)
      {
         _keys = keys;
         _values = values;
         _index = -1;
      }

      /// <summary>
      /// Creates a new instance of type KeyValueEnumerator.
      /// </summary>
      public KeyValueEnumerator (TKey[] keys, ValueOfDelegate<TValue> valueOf)
      {
         _keys = keys;
         _valueOf = valueOf;
         _index = -1;
      }

      /// <summary>
      /// Resets the enumerator to it's initial state.
      /// </summary>
      public void Reset ()
      {
         _index = -1;
      }

      /// <summary>
      /// Gets the current key/value pair.
      /// </summary>
      public KeyValuePair<TKey, TValue> Current
      {
         get
         {
            KeyValuePair<TKey, TValue> keyValue;

            if (_valueOf != null)
            {
               TKey key = _keys[_index];
               keyValue = new KeyValuePair<TKey, TValue>(key, _valueOf(key));
            }
            else
            {
               keyValue = new KeyValuePair<TKey, TValue>(_keys[_index], _values[_index]);
            }

            return keyValue;
         }
      }

      /// <summary>
      /// Moves top the next item in the enumerator.
      /// </summary>
      public bool MoveNext ()
      {
         return (++_index < _keys.Length);
      }

      object IEnumerator.Current
      {
         get
         {
            return Current;
         }
      }

      void IDisposable.Dispose ()
      {
         _keys = null;
         _values = null;
         _index = -1;
      }
   }
}
