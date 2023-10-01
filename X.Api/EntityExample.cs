using Redis.OM.Modeling;
using XDataAccess;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "EntityExample" })]
public class EntityExample : Entity<EntityExample>
{
    [Searchable]
    public string? Prop1 { get; set; }

    public override IList<EntityChange> GetChanges(EntityExample compare)
    {
        return new List<EntityChange>()
        {
            new EntityChange
            {
                Name = nameof(Prop1),
                NewValue = Prop1,
                OldValue = compare?.Prop1
            }
        };
    }
}