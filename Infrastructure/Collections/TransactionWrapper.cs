//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Transactions;
using System.Collections.Generic;

namespace Infrastructure.Collections
{
   /// <summary>
   /// Provides a transactional wrapper around a class. Code by Juval Lowry.
   /// </summary>
   public class TransactionWrapper<T> : IEnlistmentNotification
   {
      private T _value;
      private T _temporaryValue;
      private Transaction _currentTransaction;
      private TransactionLock _lock;

      /// <summary>
      /// Creates a new transactional wrapper.
      /// </summary>
      public TransactionWrapper (T value)
      {
         _lock = new TransactionLock();
         _value = value;
      }

      /// <summary>
      /// Creates a new transactional wrapper.
      /// </summary>
      public TransactionWrapper (TransactionWrapper<T> transactional) : this(transactional.Value) {}

      /// <summary>
      /// Creates a new transactional wrapper.
      /// </summary>
      public TransactionWrapper() : this(default(T)) {}

      static TransactionWrapper()
      {
         ResourceManager.ConstrainType(typeof(T));
      }

      void IEnlistmentNotification.Commit(Enlistment enlistment)
      {
         IDisposable disposable = _value as IDisposable;
         if(disposable != null)
         {
            disposable.Dispose();
         }

         _value = _temporaryValue;
         _currentTransaction = null;
         _temporaryValue= default(T);
         _lock.Unlock();
         enlistment.Done();
      }
      void IEnlistmentNotification.InDoubt(Enlistment enlistment)
      {
         _lock.Unlock();
         enlistment.Done();
      }
      void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
      {
         preparingEnlistment.Prepared();
      }

      void IEnlistmentNotification.Rollback(Enlistment enlistment)
      {
         _currentTransaction = null;

         IDisposable disposable = _temporaryValue as IDisposable;
         if(disposable != null)
         {
            disposable.Dispose();
         }

         _temporaryValue = default(T);
         _lock.Unlock();
         enlistment.Done();
      }
      void Enlist(T t)
      {
         _currentTransaction = Transaction.Current;
         _currentTransaction.EnlistVolatile(this,EnlistmentOptions.None);
         _temporaryValue = ResourceManager.Clone(t);
      }
      void SetValue(T t)
      {
         _lock.Lock();
         if(_currentTransaction == null)
         {
            if(Transaction.Current == null)
            {
               _value = t;
               return;
            }
            else
            {
               Enlist(t);
               return;
            }
         }
         else
         {
            //Must have acquired the lock
            _temporaryValue = t;
         }
      }
      T GetValue()
      {
         _lock.Lock();
         if(_currentTransaction == null)
         {
            if(Transaction.Current == null)
            {
               return _value;
            }
            else
            {
               Enlist(_value); 
            }
         }
         //Must have acquired the lock

         return _temporaryValue; 
      }

      /// <summary>
      /// Gets/sets the wrapped object.
      /// </summary>
      public T Value
      {
         get
         {
            return GetValue();
         }
         set
         {
            SetValue(value);
         }
      }

      /// <summary>
      /// Overloaded cast.
      /// </summary>
      public static implicit operator T(TransactionWrapper<T> transactional)
      {
         return transactional.Value;
      }

      /// <summary>
      /// Overloaded equals.
      /// </summary>
      public static bool operator == (TransactionWrapper<T> t1, TransactionWrapper<T> t2)
      {
         // Is t1 and t2 null (check the value as well).
         bool t1Null = (Object.ReferenceEquals(t1,null) || t1.Value == null);
         bool t2Null = (Object.ReferenceEquals(t2,null) || t2.Value == null);

         // If they are both null, return true.
         if(t1Null && t2Null)
         {
            return true;
         }

         // If one is null, return false.
         if(t1Null || t2Null)
         {
            return false;
         }
         return EqualityComparer<T>.Default.Equals(t1.Value,t2.Value);
      }

      /// <summary>
      /// Overloaded equals.
      /// </summary>
      public static bool operator == (TransactionWrapper<T> t1, T t2)
      {
         // Is t1 and t2 null (check the value as well).
         bool t1Null = (Object.ReferenceEquals(t1,null) || t1.Value == null);
         bool t2Null = t2 == null;

         // If they are both null, return true.
         if(t1Null && t2Null)
         {
            return true;
         }

         // If one is null, return false.
         if(t1Null || t2Null)
         {
            return false;
         }
         return EqualityComparer<T>.Default.Equals(t1.Value,t2);
      }

      /// <summary>
      /// Overloaded equals.
      /// </summary>
      public static bool operator == (T t1, TransactionWrapper<T> t2)
      {
         // Is t1 and t2 null (check the value as well)
         bool t1Null = t1 == null;
         bool t2Null = (Object.ReferenceEquals(t2,null) || t2.Value == null);

         // If they are both null, return true.
         if(t1Null && t2Null)
         {
            return true;
         }

         // If one is null, return false.
         if(t1Null || t2Null)
         {
            return false;
         }
         return EqualityComparer<T>.Default.Equals(t1,t2.Value);
      }

      /// <summary>
      /// Overloaded not equals.
      /// </summary>
      public static bool operator != (T t1, TransactionWrapper<T> t2)
      {
         return ! (t1 == t2);
      }

      /// <summary>
      /// Overloaded not equals.
      /// </summary>
      public static bool operator != (TransactionWrapper<T> t1, T t2)
      {
         return !(t1 == t2);
      }

      /// <summary>
      /// Overloaded not equals.
      /// </summary>
      public static bool operator!=(TransactionWrapper<T> t1,TransactionWrapper<T> t2)
      {
         return !(t1 == t2);
      }

      /// <summary>
      /// Gets the hashcode of the wrapped obejct.
      /// </summary>
      public override int GetHashCode()
      {
         return Value.GetHashCode();
      }

      /// <summary>
      /// Determines if the specified object equals the wrapped object.
      /// </summary>
      public override bool Equals(object obj)
      {
         return Value.Equals(obj);
      }
   }
}