namespace XDataAccess;

public class Clock : IClock
{
    public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.Now;
    public DateTime Now { get; } = DateTime.Now;
    public DateTime ProcessDate { get; } = DateTime.Today;
}

public interface IClock
{
    public DateTimeOffset DateTimeOffset { get; set; }
    public DateTime Now { get; }
    public DateTime ProcessDate { get; }
}