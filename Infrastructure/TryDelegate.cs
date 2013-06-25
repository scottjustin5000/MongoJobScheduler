using System;
using System.Diagnostics;

namespace Infrastructure
{
    /// <summary>
    /// 	Provides delegate automation with exception sensitivity.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// 	Gets the result of the specified getter.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "getter">The getter.</param>
        /// <param name = "handler">The error handler.</param>
        /// <returns></returns>
        public static T Get<T>(Func<T> getter, Action<Exception> handler = null)
        {
            handler = handler ?? HandleErrors.DefaultStrategy;

            try
            {
                return getter();
            }
            catch (Exception ex)
            {
                Do(() => handler(ex));
            }

            return default(T);
        }

        /// <summary>
        /// 	Does the specified action.
        /// </summary>
        /// <param name = "action">The action.</param>
        /// <param name = "handler">The error handler.</param>
        public static void Do(Action action, Action<Exception> handler = null)
        {
            handler = handler ?? HandleErrors.DefaultStrategy;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Do(() => handler(ex));
            }
        }

        /// <summary>
        /// Defines error-handling strategies used by <see cref="Try"/>.
        /// </summary>
        public static class HandleErrors
        {
            static HandleErrors()
            {
                DefaultStrategy = error => Debug.WriteLine(error.ToString());
            }

            /// <summary>
            /// Gets or sets the default strategy.
            /// </summary>
            /// <value>The default strategy.</value>
            public static Action<Exception> DefaultStrategy { get; set; }
        }
    }
}
