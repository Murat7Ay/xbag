namespace XDataAccess;

public interface IClock
{
    public DateTimeOffset DateTimeOffset { get; set; }
    public DateTime Now { get; }
    public DateTime ProcessDate { get; }
}