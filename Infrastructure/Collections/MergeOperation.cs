using System;
using System.Collections.Generic;
using System.Text;
namespace Infrastructure.Collections
{
   /// <summary>
   /// Defines the different types of set operations.
   /// </summary>
   public enum MergeOperation
   {
      /// <summary>
      /// The set of items in that are in A and B.
      /// </summary>
      Intersection,

      /// <summary>
      /// The set of items in that are in A or B.
      /// </summary>
      Union,

      /// <summary>
      /// The set of items in that are in A and not in B.
      /// </summary>
      Difference
   }
}
