namespace DynamicCodeExecution.Core;

public class ParameterArgument
{
    public ParameterArgument(string key, object value = null)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public object Value { get; }
}

public class CallArguments
{
    public CallArguments(string methodName = null)
    {
        MethodName = methodName;
    }

    public string MethodName { get; }
}