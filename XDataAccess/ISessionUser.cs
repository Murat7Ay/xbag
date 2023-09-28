namespace XDataAccess;

public interface ISessionUser
{
    string? Id { get;  set; }
    string? Ip { get;  set;}
    string? Host { get;  set;}
}