using Redis.OM.Modeling;

namespace XDataAccess;


[Document(StorageType = StorageType.Json, Prefixes = new[] { "EntityHistory" })]
[Serializable]
public class EntityHistory
{
    [RedisIdField]
    [Indexed]
    public string? Id { get; set; }
    public string? EntityName { get; set; }
    [Indexed]
    public string? EntityId { get; set; }    
    public List<EntityChange> Changes { get; set; } = new();
    public string? TraceId { get; set; }
    public DateTime? ChangeDate { get; set; }
    public string? UserId { get; set; }
    public string? Ip { get; set; }
    public string? HostName { get; set; }
}