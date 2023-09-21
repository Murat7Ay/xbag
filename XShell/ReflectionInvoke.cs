using System.Reflection;
using XInfrastructure;

namespace XShell;

public class ReflectionInvoke : IInvokeMethod
{
    private readonly List<XParameterMethodInfo> _xParameterMethodInfos;
    public ReflectionInvoke()
    {
        _xParameterMethodInfos = XDetector.DetectMethods();
    }
    public XBag Invoke(string methodName, XBag xBag)
    {
        var xParameterMethodInfo = _xParameterMethodInfos.FirstOrDefault(x => x.MethodInfo.Name == methodName);
        if (xParameterMethodInfo is null)
            throw new ArgumentNullException(methodName);
        MethodInfo methodInfo = GetMethodInfo(xParameterMethodInfo);
        var result = methodInfo.Invoke(null, new object[] { xBag });
        if (result is not XBag bag)
        {
            throw new InvalidCastException();
        }
        return bag;
    }
    
    private  MethodInfo GetMethodInfo(XParameterMethodInfo xParameterMethodInfo)
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
}
