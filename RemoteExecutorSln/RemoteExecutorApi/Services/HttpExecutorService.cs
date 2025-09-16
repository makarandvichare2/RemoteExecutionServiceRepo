using FluentValidation;
using FluentValidation.Results;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RemoteExecutorGateWayApi.Services
{
    public class HttpExecutorService : IHttpExecutorService
    {
        private readonly AbstractValidator<HttpExecutorRequest> validator;
        public HttpExecutorService(AbstractValidator<HttpExecutorRequest> validator) { 
            this.validator = validator;
        }

        public async Task<ExecutorResponse> ExecuteAsync(HttpExecutorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            string responseBody = string.Empty;
            string status = string.Empty;
            var attemptSummary = new AttemptSummary();
            var context = new Context { ["AttemptSummary"] = attemptSummary };
            try
            {
                (responseBody, status) = await ExecuteRequestWithRetryAndCircuitBreak(request, context);
            }
            catch (BrokenCircuitException ex)
            {
                return CreateExecutorResponse(
                    request,
                    new { errorMessage = "Circuit breaker is open. Please try again later." },
                    "failed",
                    attemptSummary);
            }
            catch (Exception ex)
            {
                return CreateExecutorResponse(request, new { errorMessage = ex.Message }, "failed", attemptSummary);
            }
            return CreateExecutorResponse(request, JsonSerializer.Deserialize<dynamic>(responseBody),"status", attemptSummary);
        }

        private static ExecutorResponse CreateExecutorResponse(HttpExecutorRequest request,dynamic result,string status, AttemptSummary attemptSummary)
        {
            return new ExecutorResponse
            {
                CorrelationId = request.CorrelationId,
                Status = status,
                Result = result,
                AttemptSummary = attemptSummary
            };
        }

        private async Task<(string responseBody, string status)> ExecuteRequestWithRetryAndCircuitBreak(HttpExecutorRequest request, Context context)
        {

            var summary = context["AttemptSummary"] as AttemptSummary;
            var currentAttempt = new Attempt { AttemptNumber = summary.Attempts.Count + 1, StartTimeUtc = DateTime.UtcNow };
            summary.Attempts.Add(currentAttempt);

            var resiliencePolicy = SetResiliencePolicy(request, context);
            var executorResult = await resiliencePolicy.ExecuteAsync(async () =>
            {
                using var httpClient = new HttpClient();
                StringContent content = null;
                string url = request.RequestBody.Url;
                if (CheckBodyExists(request.RequestBody.Body))
                {
                    content = new StringContent(request.RequestBody.Body.GetRawText(), Encoding.UTF8, "application/json");
                }

                if (request.RequestBody.QueryParams != null && request.RequestBody.QueryParams.Count > 0)
                {
                    url = url + GetQueryString(request.RequestBody.QueryParams);
                }

                try
                {
                    var result = await httpClient.PostAsync(url, content);
                    return result;
                }
                catch(Exception ex) 
                {
                    currentAttempt.Status = "failure";
                    currentAttempt.ErrorMessage = ex.Message;
                    throw; 
                }
                finally
                {
                    currentAttempt.EndTimeUtc = DateTime.UtcNow;
                }
            });

            executorResult.EnsureSuccessStatusCode();
            var status = "sucess";
            var responseBody = await executorResult.Content.ReadAsStringAsync();

            return (responseBody, status);
        }

        private bool CheckBodyExists(JsonElement body)
        {
            if (body.ValueKind == JsonValueKind.Undefined)
            {
                return false;
            }
            else if (body.ValueKind == JsonValueKind.Null)
            {
                return false;
            }
            return true;
        }

        private Polly.Wrap.AsyncPolicyWrap<HttpResponseMessage> SetResiliencePolicy(HttpExecutorRequest request, Context context)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(request.ExecutionPolicy.MaxRetries, retryAttempt =>
                    TimeSpan.FromMilliseconds(request.ExecutionPolicy.DelayTimeoutInMiliSeconds * Math.Pow(2, retryAttempt)),
                    onRetry: (httpResponse, timespan, retryAttempt, policyContext) =>
                    {
                        var summary = context["AttemptSummary"] as AttemptSummary;
                        var attempt = new Attempt { AttemptNumber = retryAttempt + 1, Status = "transient_failure", StartTimeUtc = DateTime.UtcNow,ErrorMessage = httpResponse.Exception.Message };
                        summary?.Attempts.Add(attempt);
                    });

            var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(request.ExecutionPolicy.MaxEventBeforeBreak,
                TimeSpan.FromSeconds(request.ExecutionPolicy.BreakTimeoutInSeconds)
                );

            return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy);
        }

        private string GetQueryString(Dictionary<string, string> queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return string.Empty;
            }

            var encodedPairs = queryParams.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}");
            return "?" + string.Join("&", encodedPairs);
        }
    }
}
