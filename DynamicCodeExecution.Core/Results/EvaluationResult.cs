using DynamicCodeExecution.Core.Errors;

namespace DynamicCodeExecution.Core.Results;

public class EvaluationResult
{
    private EvaluationResult(IEnumerable<DynamicScriptCompilationError> errors = null)
    {
        Errors = errors ?? new List<DynamicScriptCompilationError>();
    }

    public bool Success => !Errors.Any();
    public IEnumerable<DynamicScriptCompilationError> Errors { get; }

    public static EvaluationResult WithErrors(IEnumerable<DynamicScriptCompilationError> errors)
    {
        return new(errors);
    }

    public static EvaluationResult Ok()
    {
        return new();
    }
}