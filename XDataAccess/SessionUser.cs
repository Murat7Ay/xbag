namespace XDataAccess;

public abstract class SessionUser : ISessionUser
{
    public string? Id { get; set; }
    public string? Ip { get; set; }
    public string? Host { get; set; }
}