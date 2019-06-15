using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Domain
{
    /// <summary>
    /// The operation relates to a domain concept that is not a natural part of an Entity or Value Object.
    /// The interface is defined in terms of other elements of the domain model.
    /// The operation is stateless.
    /// </summary>
    public interface IDomainBusiness : ITransientDependency
    {
    }
}

