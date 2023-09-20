namespace DynamicCodeExecution.Core.Errors;

public class DynamicScriptCompilationError
{
    public DynamicScriptCompilationError(string errorMessage, int fromLine, int toLine, int fromCharacter, int length)
    {
        ErrorMessage = errorMessage;
        FromLine = fromLine;
        ToLine = toLine;
        FromCharacter = fromCharacter;
        Length = length;
    }

    public string ErrorMessage { get; }
    public int FromLine { get; }
    public int ToLine { get; }
    public int FromCharacter { get; }
    public int Length { get; }
}