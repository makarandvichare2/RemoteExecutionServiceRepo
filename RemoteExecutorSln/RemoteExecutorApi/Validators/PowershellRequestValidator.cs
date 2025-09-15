using FluentValidation;
using RemoteExecutorGateWayApi.ViewModels.Requests;

namespace RemoteExecutorApi.API.Controllers;
public class PowershellRequestValidator : AbstractValidator<PowerShellExecutorRequest>
{
    public PowershellRequestValidator()
    {
        RuleFor(x => x.CorrelationId)
  .NotNull()
  .WithMessage("CorrelationId is required.");

        RuleFor(x => x.ExecutionPolicy)
          .NotNull()
          .WithMessage("ExecutionPolicy is required.");

        RuleFor(x => x.ExecutionPolicy.MaxRetries)
          .GreaterThan(0)
          .WithMessage("MaxRetries must be greater than zero.")
          .LessThan(5)
          .WithMessage("MaxRetries must be less than 5.");
        RuleFor(x => x.RequestBody)
         .NotNull()
          .WithMessage("RequestBody is required.");
    }
}
