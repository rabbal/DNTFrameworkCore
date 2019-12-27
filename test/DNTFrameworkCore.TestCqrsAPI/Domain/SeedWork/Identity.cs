namespace DNTFrameworkCore.TestCqrsAPI.Domain.SeedWork
{
    public abstract class Identity<T> where T : IEquatable<T>
    {
//        public T Value { get; }
//
//        protected Identity(T value)
//        {
//            Value = value;
//        }
//        private int? _hashCode;
//
//        public override int GetHashCode()
//        {
//            if (IsTransient()) return base.GetHashCode();
//
//            if (!_hashCode.HasValue)
//                _hashCode = (GetType().ToString() + Value).GetHashCode() ^ 31; // XOR for random distribution
//
//            return _hashCode.Value;
//        }
//        public override bool Equals(object obj)
//        {
//            if (obj == null || !(obj is Identity<T> other)) return false;
//
//            if (GetType() != other.GetType()) return false;
//
//            if (IsTransient() || other.IsTransient()) return false;
//
//            return Value.Equals(other.Value);
//        }
//
//        public override string ToString()
//        {
//            return $"[{GetType().Name} : {Value}]";
//        }
//
//        public static bool operator ==(Identity<T> left, Identity<T> right)
//        {
//            return Equals(left, right);
//        }
//
//        public static bool operator !=(Identity<T> left, Identity<T> right)
//        {
//            return !(left == right);
//        }

    }
}