namespace DNTFrameworkCore.Specifications
{
    /// <summary>
    /// Represents the base class for composite specifications.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the specification is applied.</typeparam>
    public abstract class CompositeSpecification<T> : Specification<T>
    {
        /// <summary>
        /// Gets the first specification.
        /// </summary>
        public Specification<T> Left { get; }

        /// <summary>
        /// Gets the second specification.
        /// </summary>
        public Specification<T> Right { get; }

        protected CompositeSpecification(Specification<T> left, Specification<T> right)
        {
            Left = left;
            Right = right;
        }
    }
}