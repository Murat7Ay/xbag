namespace XDataAccess;

[Serializable]
public class EntityChange
{
    public string? Name { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}