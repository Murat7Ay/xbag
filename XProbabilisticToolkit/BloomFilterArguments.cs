namespace XProbabilisticToolkit;

public class BloomFilterArguments
{
    public string? Key { get; set; }
    public string? Item { get; set; }
    public bool NoCreate { get; set; }
    public long? Capacity { get; set; }
    public decimal? Error { get; set; }
    public bool NonScaling { get; set; }
    public int? Expansion { get; set; }
    public ISet<string>? Items { get; set; }
}