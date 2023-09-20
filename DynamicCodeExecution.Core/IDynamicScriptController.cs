using DynamicCodeExecution.Core.Results;

namespace DynamicCodeExecution.Core;

public interface IDynamicScriptController<T, C>
    where T : IDynamicScriptParameter
    where C : CallArguments
{
    EvaluationResult Evaluate(T t);
    ExecutionResult Execute(C callingArgs = null, List<ParameterArgument> methodArgs = null);
}