namespace Certificate.Domain.Core.Models
{
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        #region Protected Methods

        protected abstract bool EqualsCore(T otherObject);

        protected abstract int GetHashCodeCore();

        #endregion Protected Methods

        #region Public Methods

        public override bool Equals(object? otherObject)
        {
            T? _valueObject = otherObject as T;

            return _valueObject is not null && EqualsCore(_valueObject);
        }

        public override int GetHashCode() => GetHashCodeCore();

        public override string ToString() => GetType().Name;

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b) => !(a == b);

        #endregion Public Methods
    }
}
