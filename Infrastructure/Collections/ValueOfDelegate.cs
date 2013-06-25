using System;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides a callback for creating indexes of objects.
   /// </summary>
   public delegate object ValueOfDelegate (object item);

   /// <summary>
   /// Provides a callback for creating indexes of objects.
   /// </summary>
   public delegate TValue ValueOfDelegate<TValue> (object item);

   /// <summary>
   /// Provides a callback for creating indexes of objects.
   /// </summary>
   public delegate TValue ValueOfDelegate<TItem, TValue> (TItem item);
}
