using FluentValidation;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Enums;


namespace RemoteExecutorApi.API.Validators;
public class ExecutorJsonRequestValidator : AbstractValidator<ExecutorJsonRequest>
{
    public ExecutorJsonRequestValidator()
    {
        RuleFor(x => x.ExecutorType)
          .Must(executionType => Enum.IsDefined(typeof(ExecutionTypeEnum), executionType))
          .WithMessage("ExecutionType is not valid.");

        RuleFor(x => x.RequestBody.GetRawText())
            .MaximumLength(600)
            .WithMessage("RequestBody is too large.");
    }
}
