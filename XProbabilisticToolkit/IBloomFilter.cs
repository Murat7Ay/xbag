namespace XProbabilisticToolkit;

public interface IBloomFilter
{
    /// <summary>
    /// Adds a single item to the specified Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>1 if the item was likely added, 0 if it was already likely present.</returns>
    int AddItem(string filterName, string item);
    
    /// <summary>
    /// Adds multiple items to the specified Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <param name="items">The items to add.</param>
    /// <returns>A dictionary mapping each item to the result of its addition (1 for likely added, 0 for likely already present).</returns>
    IDictionary<string, int> AddItems(string filterName, ISet<string> items);
    
    /// <summary>
    /// Returns an estimated count of unique items in the Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <returns>The estimated cardinality (number of unique items).</returns>
    int EstimateCardinality(string filterName);
    /// <summary>
    /// Checks whether a single item is likely present in the Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <param name="item">The item to check.</param>
    /// <returns>1 if the item is likely present, 0 if it's likely absent.</returns>
    int CheckExistence(string filterName, string item);
    /// <summary>
    /// Checks whether multiple items are likely present in the Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <param name="items">The items to check.</param>
    /// <returns>A dictionary mapping each item to its existence check result (true for likely present, false for likely absent).</returns>
    IDictionary<string, int> CheckExistence(string filterName, ISet<string> items);
    /// <summary>
    /// Returns metadata information about the Bloom filter.
    /// </summary>
    /// <param name="filterName">The name of the Bloom filter.</param>
    /// <returns>A dictionary containing filter information such as type, capacity, and error rate.</returns>
    IDictionary<string, string> Info(string filterName);
    /// <summary>
    /// Creates a new Bloom filter with specified parameters, enabling flexibility in configuration.
    /// Accepts a BloomFilterArguments object to provide granular control over filter settings.
    /// Returns a dictionary with detailed results for troubleshooting and informative feedback.
    /// </summary>
    /// <param name="bloomFilterArguments">A BloomFilterArguments object containing configuration options.</param>
    /// <returns>A dictionary with keys indicating success or failure, along with relevant details.</returns>
    IDictionary<string, int> CreateFilter(BloomFilterArguments bloomFilterArguments);
}