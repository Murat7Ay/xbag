using DynamicCodeExecution.Core.Errors;

namespace DynamicCodeExecution.Core.Results;

public class ExecutionResult
{
    private ExecutionResult(List<DynamicScriptExecutionError> errors = null)
    {
        Results = new List<ExecutionResultEntry>();
        Errors = errors ?? new List<DynamicScriptExecutionError>();
    }

    public List<ExecutionResultEntry> Results { get; }

    public List<DynamicScriptExecutionError> Errors { get; }

    public bool Success => !Errors.Any();

    public object ReturnValue => Results.FirstOrDefault(x => x is MethodReturnValue)?.Value;

    public object this[string key] => GetValue<object>(key);

    public static ExecutionResult WithError(Exception exception)
    {
        return new ExecutionResult(new List<DynamicScriptExecutionError>
        {
            new(exception.Message)
        });
    }

    public static ExecutionResult Ok()
    {
        return new ExecutionResult();
    }

    public void Add(ExecutionResultEntry entry)
    {
        Results.Add(entry);
    }

    public T ReturnValueOf<T>()
    {
        return (T)Results.FirstOrDefault(x => x is MethodReturnValue)?.Value;
    }

    public T GetValue<T>(string key)
    {
        return (T)Results.FirstOrDefault(x => x.OutputName == key)?.Value;
    }
}

public class ExecutionResultEntry
{
    public ExecutionResultEntry(string outputName, object value)
    {
        OutputName = outputName;
        Value = value;
    }

    public string OutputName { get; }
    public object Value { get; }
}

public class MethodReturnValue : ExecutionResultEntry
{
    public MethodReturnValue(object value) : base("*RETURN_TYPE*", value)
    {
    }
}