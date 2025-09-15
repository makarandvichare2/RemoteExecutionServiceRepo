using FluentValidation;
using RemoteExecutorGateWayApi.Commands;

namespace BikeShop.API.Controllers;
public class PowershellRequestValidator : AbstractValidator<PowershellRequestCommand>
{
    public PowershellRequestValidator()
    {
        RuleFor(x => x.CommandRequest.CorrelationId)
  .NotNull()
  .WithMessage("CorrelationId is required.");

        RuleFor(x => x.CommandRequest.ExecutionPolicy)
          .NotNull()
          .WithMessage("ExecutionPolicy is required.");

        RuleFor(x => x.CommandRequest.ExecutionPolicy.MaxRetries)
          .GreaterThan(0)
          .WithMessage("MaxRetries must be greater than zero.")
          .LessThan(5)
          .WithMessage("MaxRetries must be less than 5.");
        RuleFor(x => x.CommandRequest.RequestBody)
         .NotNull()
          .WithMessage("RequestBody is required.");
    }
}
