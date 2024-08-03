using Redis.OM.Modeling;
using XDataAccess;

[Document(StorageType = StorageType.Json,IndexName = "idx:user", Prefixes = new[] { "User" })]
public class UserEntity : Entity<UserEntity> 
{
    [Indexed(CaseSensitive = false)] 
    public string? Name { get; set; }
    [Indexed] 
    public string? Password { get; set; }
    public string? Role { get; set; }
}