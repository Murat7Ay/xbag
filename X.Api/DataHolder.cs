using System.Text.Json.Nodes;
using Redis.OM.Modeling;
using XDataAccess;

namespace X.Api;

[Document(StorageType = StorageType.Json,IndexName = "idx:data", Prefixes = new[] { "DataHolder" })]
public class DataHolder : Entity<DataHolder>
{
    public JsonObject Data { get; set; } 
}