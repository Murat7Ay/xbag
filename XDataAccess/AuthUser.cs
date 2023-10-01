namespace XDataAccess;

public  class AuthUser : IAuthUser
{
    public string? Id { get; set; } = "TEST_ID";
    public string? Ip { get; set; } = "TEST_IP";
    public string? Host { get; set; } = "TEST_HOST";
    public string? TraceId { get; set; } = "TEST_TRACEID";
}