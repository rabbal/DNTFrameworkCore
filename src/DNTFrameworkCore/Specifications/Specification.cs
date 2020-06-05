using System;
using System.Linq.Expressions;

namespace DNTFrameworkCore.Specifications
{
    /// <summary>
    /// Represents the base class for specifications.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the specification is applied.</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value which indicates whether the specification
        /// is satisfied by the given object.
        /// </summary>
        /// <param name="instance">The object to which the specification is applied.</param>
        /// <returns>True if the specification is satisfied, otherwise false.</returns>
        public virtual bool SatisfiedBy(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            return ToExpression().Compile()(instance);
        }

        /// <summary>
        /// Gets the LINQ expression which represents the current specification.
        /// </summary>
        /// <returns>The LINQ expression.</returns>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Implicitly converts a specification to expression.
        /// </summary>
        /// <param name="specification"></param>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));
            
            return specification.ToExpression();
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that both the current and
        /// the given specification must be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public Specification<T> And(Specification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return new AndSpecification<T>(this, specification);
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that either the current or
        /// the given specification should be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public Specification<T> Or(Specification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return new OrSpecification<T>(this, specification);
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that the current specification
        /// should be satisfied by the given object but the specified specification should not.
        /// </summary>
        /// <param name="specification">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public Specification<T> AndNot(Specification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return new AndNotSpecification<T>(this, specification);
        }

        /// <summary>
        /// Reverses the current specification instance and returns a specification which represents
        /// the semantics opposite to the current specification.
        /// </summary>
        /// <returns>The reversed specification instance.</returns>
        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
    }
}