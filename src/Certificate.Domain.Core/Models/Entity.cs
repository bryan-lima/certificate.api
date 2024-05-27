using System;

namespace Certificate.Domain.Core.Models
{
    public abstract class Entity : ValueObject<Entity>
    {
        #region Protected Constructors

        protected Entity()
        {
            CreatedDate = DateTime.Now;
            IsDeleted = false;
        }

        #endregion Protected Constructors

        #region Public Properties

        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override bool EqualsCore(Entity otherObject)
        {
            if (ReferenceEquals(objA: this, objB: otherObject))
                return true;

            if (otherObject is null)
                return false;

            return Id.Equals(otherObject.Id);
        }

        protected override int GetHashCodeCore() => (GetType().GetHashCode() * 503) + Id.GetHashCode();

        #endregion Protected Methods
    }
}
