using System;
using System.Collections.Generic;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.MultiTenancy
{
    public class TenantContext : IDisposable
    {
        private bool _disposed;

        public TenantContext(TenantInfo tenant)
        {
            Guard.ArgumentNotNull(tenant, nameof(tenant));

            Tenant = tenant;
            Properties = new Dictionary<string, object>();
        }
    
        public string Id { get; } = Guid.NewGuid().ToString();

        public TenantInfo Tenant { get; }

        public IDictionary<string, object> Properties { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TenantContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var prop in Properties)
                {
                    TryDisposeProperty(prop.Value as IDisposable);
                }
                
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

        private static void TryDisposeProperty(IDisposable obj)
        {
            if (obj == null) return;

            try
            {
                obj.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}