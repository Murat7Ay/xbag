using Redis.OM.Modeling;

namespace XDataAccess;

[Serializable]
public abstract class Entity
{
    [RedisIdField] [Indexed] public string? Id { get; set; }
    [Indexed]
    public bool IsDeleted { get; set; }
    [Indexed]
    public bool IsActive { get; set; }
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

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Id = {Id}";
    }
}