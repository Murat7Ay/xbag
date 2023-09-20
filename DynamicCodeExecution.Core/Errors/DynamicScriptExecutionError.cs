namespace DynamicCodeExecution.Core.Errors;

public class DynamicScriptExecutionError
{
    public DynamicScriptExecutionError(string message)
    {
        Message = message;
    }

    public string Message { get; }
}