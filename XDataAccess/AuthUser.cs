namespace XDataAccess;

public abstract class AuthUser : IAuthUser
{
    public string? Id { get; set; }
    public string? Ip { get; set; }
    public string? Host { get; set; }
    public string? TraceId { get; set; }
}