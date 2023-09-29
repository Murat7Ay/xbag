namespace XDataAccess;

public interface IAuthUser
{
    string? Id { get;  set; }
    string? Ip { get;  set;}
    string? Host { get;  set;}
}