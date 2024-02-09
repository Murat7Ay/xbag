using Redis.OM.Modeling;
using XDataAccess;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "EntityExample" })]
public class EntityExample : Entity<EntityExample>
{
    [Searchable]
    public string? Prop1 { get; set; }
    [Indexed]
    public List<string> Prop3 { get; set; }
    [Indexed]
    public ExampleInsideClass PropExample { get; set; }
    
    
    [Indexed]
    public int IntegerProperty { get; set; }

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


public class ExampleInsideClass
{
    [Indexed]
    public string? Prop2 { get; set; }
}