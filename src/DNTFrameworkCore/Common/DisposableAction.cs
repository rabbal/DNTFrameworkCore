using System;
using System.Threading;

namespace DNTFrameworkCore.Common
{
    /// <summary>
    /// This class can be used to provide an action when
    /// Dispose method is called.
    /// </summary>
    public class DisposableAction : Disposable
    {
        public static readonly DisposableAction Empty = new DisposableAction(null);

        private Action _action;

        /// <summary>
        /// Creates a new <see cref="DisposableAction"/> object.
        /// </summary>
        /// <param name="action">Action to be executed when this object is disposed.</param>
        public DisposableAction(Action action)
        {
            _action = action;
        }

        protected override void DisposeExplicit()
        {
            // Interlocked prevents multiple execution of the _action.
            var action = Interlocked.Exchange(ref _action, null);
            action?.Invoke();
        }
    }

    /// <summary>
    /// This class can be used to provide an action when
    /// Dispose method is called.
    /// </summary>
    public class DisposableAction<T> : Disposable
        where T : DisposableAction<T>
    {
        private Action<T> _action;

        /// <summary>
        /// Creates a new <see cref="DisposableAction"/> object.
        /// </summary>
        /// <param name="action">Action to be executed when this object is disposed.</param>
        public DisposableAction(Action<T> action)
        {
            _action = action;
        }

        protected override void DisposeExplicit()
        {
            // Interlocked prevents multiple execution of the _action.
            var action = Interlocked.Exchange(ref _action, null);
            action?.Invoke((T) this);
        }
    }
}