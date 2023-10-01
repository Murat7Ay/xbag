using XInfrastructure;

namespace XShell;

public static class InvokeMethodFactory
{
    public static IInvokeMethod Create(string methodName)
    {
        return new ReflectionInvoke();
    }
}