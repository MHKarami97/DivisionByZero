namespace Entities.Common
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
        public int Version { get; set; }
        public int VersionStatus { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}
