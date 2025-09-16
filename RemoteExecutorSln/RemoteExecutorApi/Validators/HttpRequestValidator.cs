using FluentValidation;
using RemoteExecutorGateWayApi.ViewModels.Requests;


namespace RemoteExecutorApi.API.Validators;
public class HttpRequestValidator : AbstractValidator<HttpExecutorRequest>
{
    public HttpRequestValidator()
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
          .LessThan(15)
          .WithMessage("MaxRetries must be less than 15.");

        RuleFor(x => x.RequestBody)
         .NotNull()
          .WithMessage("RequestBody is required.");

        RuleFor(x => x.ExecutionPolicy.MaxEventBeforeBreak)
          .GreaterThan(0)
          .WithMessage("MaxEventBeforeBreak must be greater than zero.")
          .LessThan(15)
          .WithMessage("MaxEventBeforeBreak must be less than 15.");

        RuleFor(x => x.ExecutionPolicy.DelayTimeoutInMiliSeconds)
          .GreaterThan(0)
          .WithMessage("DelayTimeoutInMiliSeconds must be greater than zero.")
          .LessThan(3000)
          .WithMessage("DelayTimeoutInMiliSeconds must be less than 20.");

        RuleFor(x => x.ExecutionPolicy.BreakTimeoutInSeconds)
          .GreaterThan(0)
          .WithMessage("BreakTimeoutInSeconds must be greater than zero.")
          .LessThan(20)
          .WithMessage("BreakTimeoutInSeconds must be less than 20.");

    }
}
