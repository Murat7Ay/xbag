namespace XProbabilisticToolkit;

public interface ICuckooFilter
{
    /// <summary>
    /// Adds an element to the filter. **(Redis command: CF.ADD)**
    /// </summary>
    /// <param name="element">The element to add.</param>
    /// <returns>True if the element was added successfully, false otherwise.</returns>
    bool Add(string element);

    /// <summary>
    /// Adds an element to the filter only if it doesn't already exist. **(Redis command: CF.ADDNX)**
    /// </summary>
    /// <param name="element">The element to add.</param>
    /// <returns>True if the element was added successfully (didn't exist previously), false otherwise.</returns>
    bool AddIfNotExists(string element);

    /// <summary>
    /// Adds multiple elements to the filter. **(Redis command: CF.INSERT)**
    /// </summary>
    /// <param name="elements">The collection of elements to add.</param>
    /// <returns>The number of elements successfully added.</returns>
    int Add(IEnumerable<string> elements);

    /// <summary>
    /// Adds multiple elements to the filter only if they don't already exist. **(Redis command: CF.INSERTNX)**
    /// </summary>
    /// <param name="elements">The collection of elements to add.</param>
    /// <returns>The number of elements successfully added (didn't exist previously).</returns>
    int AddIfNotExists(IEnumerable<string> elements);

    /// <summary>
    /// Checks if an element exists in the filter. **(Redis command: CF.EXISTS)**
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns>True if the element might exist (potential false positive), false otherwise.</returns>
    bool MightContain(string element);

    /// <summary>
    /// Checks if multiple elements exist in the filter. **(Redis command: CF.MEXISTS)**
    /// </summary>
    /// <param name="elements">The collection of elements to check.</param>
    /// <returns>A dictionary mapping each element to a boolean indicating its potential presence (true) or confirmed absence (false).</returns>
    IDictionary<string, bool> MightContain(IEnumerable<string> elements);

    /// <summary>
    /// Removes an element from the filter. **(Redis command: CF.DELETE)**
    /// </summary>
    /// <param name="element">The element to remove.</param>
    /// <returns>True if the element was removed successfully, false otherwise.</returns>
    bool Remove(string element);

    /// <summary>
    /// Reserves space for creating a new filter with the specified capacity and fingerprint function names. **(Redis command: CF.RESERVE)**
    /// </summary>
    /// <param name="capacity">The capacity of the filter.</param>
    /// <param name="fpFunctionNames">The names of the fingerprint functions to use.</param>
    /// <returns>True if the reservation was successful, false otherwise.</returns>
    bool CreateFilter(int capacity, string[] fpFunctionNames);

    /// <summary>
    /// Gets information about the filter, such as capacity, size, and fingerprinting functions. **(Redis command: CF.INFO)**
    /// </summary>
    /// <returns>A dictionary containing filter information.</returns>
    IDictionary<string, string> Info();

    /// <summary>
    /// Estimated number of elements in the filter. **(Redis command: CF.COUNT)**
    /// </summary>
    /// <returns>Estimated number of elements.</returns>
    long EstimatedCount();
}