namespace XDataAccess;

public interface IClock
{
    public DateTime Now { get; }
    public DateTime ProcessDate { get; }
}