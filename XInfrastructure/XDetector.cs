using System.Reflection;

namespace XInfrastructure;

public static class XDetector
{
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