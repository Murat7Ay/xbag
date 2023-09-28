namespace XDataAccess;

[Serializable]
public class EntityHistory
{
    public string? Id { get; set; }
    public string? MyEntityId { get; set; }
    public List<EntityHistoryChange> Changes { get; set; } = new();
    public DateTime? ChangeDate { get; set; }
    public string? UserId { get; set; }
    public string? Ip { get; set; }
    public string? HostName { get; set; }
}