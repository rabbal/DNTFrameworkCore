using System;
using System.Reflection;

namespace DNTFrameworkCore.Common
{
    /// <summary>
    /// Provides a base implementation of a disposable object
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private bool _disposed;
        protected virtual object This => this;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeExplicit();
            }

            DisposeImplicit();

            _disposed = true;
        }

        /// <summary>
        /// Performs operations when the instance is explicitly disposed.
        /// Free other managed objects that implement IDisposable only
        /// </summary>
        protected virtual void DisposeExplicit()
        {
        }

        /// <summary>
        /// Performs operations when the instance is implicitly disposed.
        /// Release any unmanaged objects set the object references to null
        /// </summary>
        protected virtual void DisposeImplicit()
        {
        }

        protected void ThrowIfDisposed()
        {
            if (!_disposed) return;
            
            var objectName = This.GetType().GetTypeInfo().Name;
            throw new ObjectDisposedException(objectName);
        }
    }
}