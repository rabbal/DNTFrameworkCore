using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// This interface should be implemented by vendors to
    /// make auditing working.
    /// Default implementation is <see cref="SimpleLogAuditingStore"/>.
    /// </summary>
    public interface IAuditingStore : IScopedDependency
    {
        /// <summary>
        /// Should save audits to a persistent store.
        /// </summary>
        /// <param name="auditInfo">Audit informations</param>
        void Save(AuditInfo info);
    }
}