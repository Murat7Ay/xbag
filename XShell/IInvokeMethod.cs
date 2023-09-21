using XInfrastructure;

namespace XShell;

public interface IInvokeMethod
{
    public XBag Invoke(string methodName, XBag xBag);
}