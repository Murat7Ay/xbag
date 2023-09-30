using Redis.OM.Modeling;

namespace XDataAccess;

internal interface IEntity<TEntity>
{
    string? Id { get; set; }
    string? XId { get; set; }
    bool IsDeleted { get; set; }
    bool IsActive { get; set; }
    string? TraceId { get; set; }
    string? DeletedBy { get; set; }
    DateTime? DeleteDate { get; set; }
    DateTime? ModifyDate { get; set; }
    string? ModifiedBy { get; set; }
    DateTime CreateDate { get; set; }
    string? CreatedBy { get; set; }
    int EntityVersion { get; set; }
    string? Ip { get; set; }
    string? Host { get; set; }
    object?[] GetKeys();
    IList<EntityChange>  GetChanges(TEntity compare);
}

public abstract class Entity<TEntity> : IEntity<TEntity>
{
    [RedisIdField, Indexed] 
    public string? Id { get; set; }
    [Indexed]
    public string? XId { get; set; }
    [Indexed]
    public bool IsDeleted { get; set; }
    [Indexed]
    public bool IsActive { get; set; }
    [Indexed, Searchable]
    public string? TraceId { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeleteDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public string? CreatedBy { get; set; }
    public int EntityVersion { get; set; }
    public string? Ip { get; set; }
    public string? Host { get; set; }

    public virtual object?[] GetKeys()
    {
        return new object?[] { Id };
    }

    public abstract IList<EntityChange> GetChanges(TEntity compare);
    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Id = {Id}";
    }
}