namespace XProbabilisticToolkit;

public interface ISimpleHyperLogLog
{
    /// <summary>
    /// Adds elements to the HyperLogLog data structure.
    /// </summary>
    /// <param name="elements">The collection of elements to add.</param>
    void Add(IEnumerable<string> elements);

    /// <summary>
    /// Deletes the HyperLogLog key from the Redis database.
    /// </summary>
    void Delete();

    /// <summary>
    /// Estimates the cardinality (count of unique elements) added to the HyperLogLog data structure.
    /// </summary>
    /// <returns>An estimate of the cardinality.</returns>
    long EstimateCardinality();
}