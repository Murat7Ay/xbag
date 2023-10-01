using System.Reflection;

namespace XInfrastructure;

public static class XHandler
{
    public static XParameterMethodInfo GetMethodInfoByName(string methodName)
    {
        List<XParameterMethodInfo> parameterMethods = XDetector.DetectMethods();

        var filteredParameterMethods =
            parameterMethods.Where(x => !string.IsNullOrEmpty(x.MethodInfo.Name) && x.MethodInfo.Name == methodName)
                .ToList();

        if (!filteredParameterMethods.Any())
        {
            throw new Exception("Identifier not found");
        }

        if (filteredParameterMethods.Count > 1)
        {
            throw new Exception("Multiple identifiers found");
        }

        return filteredParameterMethods.First();
    }
}