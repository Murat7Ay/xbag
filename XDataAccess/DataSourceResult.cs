using System.Collections;

namespace XDataAccess;

public class DataSourceResult
{
    public IEnumerable Data { get; set; }

    public int Total { get; set; }

    public object Aggregates { get; set; }

    private static Type[] GetKnownTypes()
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.FullName.StartsWith("DynamicClasses"));

        if (assembly == null)
        {
            return new Type[0];
        }

        return assembly.GetTypes()
            .Where(t => t.Name.StartsWith("DynamicClass"))
            .ToArray();
    }
}