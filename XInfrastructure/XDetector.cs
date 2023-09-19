using System.Reflection;

namespace XInfrastructure;

public static class XDetector
{
    public static XBag InvokeMethod(XBag inBag, string methodName)
    {
        XParameterMethodInfo xParameterMethodInfo = GetMethodInfoByName(methodName);
        XBag outBag = Invoke(xParameterMethodInfo, inBag);
        return outBag;
    } 
    private static XBag Invoke(XParameterMethodInfo xParameterMethodInfo, XBag inBag)
    {
        MethodInfo methodInfo = GetMethodInfo(xParameterMethodInfo);
        object? result = methodInfo.Invoke(null, new object[] { inBag });

        if (result is not XBag bag)
        {
            throw new Exception();
        }

        return bag;
    }
    private static MethodInfo GetMethodInfo(XParameterMethodInfo xParameterMethodInfo)
    {
        Assembly? assembly = GetAssemblyByName(xParameterMethodInfo.Assembly!);

        if (assembly == null)
        {
            throw new Exception("Assembly not found");
        }

        Type? type = assembly.GetType(xParameterMethodInfo.TypeName!);
        if (type == null)
        {
            throw new Exception("Type not found");
        }

        MethodInfo? methodInfo = type.GetMethod(xParameterMethodInfo.MethodName!);
        if (methodInfo == null)
        {
            throw new Exception("Method info not found");
        }

        return methodInfo;
    }
    private static Assembly? GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
    }
    public static XParameterMethodInfo GetMethodInfoByName(string methodName)
    {
        List<XParameterMethodInfo> parameterMethods = DetectMethods();

        var filteredParameterMethods =
            parameterMethods.Where(x => !string.IsNullOrEmpty(x.MethodInfo.Name) && x.MethodInfo.Name == methodName).ToList();

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
    public static List<XParameterMethodInfo> DetectMethods()
    {
        List<XParameterMethodInfo> xParameterMethodInfos = new();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == false) continue;
                foreach (MethodInfo method in type.GetRuntimeMethods())
                {
                    if (method.IsStatic == false) continue;
                    var xMethodAttribute = method.GetCustomAttribute<XMethodAttribute>();
                    if (xMethodAttribute is null)
                        continue;
                    if (assembly == null || string.IsNullOrEmpty(type.FullName))
                        continue;
                    AssemblyName assemblyName = assembly.GetName();
                    XParameterMethodInfo xParameterMethodInfo = new()
                    {
                        Assembly = assemblyName.Name,
                        TypeName = type.FullName,
                        MethodName = method.Name,
                        MethodInfo = xMethodAttribute
                    };
                    List<XParameterAttribute> parameterAttributes = method.GetCustomAttributes<XParameterAttribute>().ToList();
                    xParameterMethodInfo.Inputs = parameterAttributes.Where(x=>x.XParameter == XParameterType.Input).ToList();
                    xParameterMethodInfo.Outputs = parameterAttributes.Where(x=>x.XParameter == XParameterType.OutPut).ToList();
                    xParameterMethodInfos.Add(xParameterMethodInfo);
                }
            }
        }

        return xParameterMethodInfos;
    }
}