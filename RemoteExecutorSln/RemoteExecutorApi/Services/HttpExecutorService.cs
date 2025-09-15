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
            DateTime startTimeUtc = DateTime.Now.ToUniversalTime();
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
            try
            {
              (responseBody, status) = await ExecuteRequestWithRetryAndCircuitBreak(request);
            }
            catch (BrokenCircuitException ex)
            {
                attemptSummary.Attempts.Add(new Attempt(attemptSummary.Attempts.Count + 1, "boken_circuit_failure", startTimeUtc, DateTime.Now.ToUniversalTime()));
            }
            catch (Exception ex)
            {
                attemptSummary.Attempts.Add(new Attempt(attemptSummary.Attempts.Count + 1, "permanent_failure", startTimeUtc, DateTime.Now.ToUniversalTime()));
            }
            return GetExecutorResponse(request, startTimeUtc, responseBody, status, attemptSummary);
        }

        private async Task<(string responseBody, string status)> ExecuteRequestWithRetryAndCircuitBreak(HttpExecutorRequest request)
        {
            var resiliencePolicy = SetResiliencePolicy(request);
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

                var result = await httpClient.PostAsync(url, content);
                return result;
            });

            executorResult.EnsureSuccessStatusCode();
            var status = "sucess";
            var responseBody = await executorResult.Content.ReadAsStringAsync();

            return (responseBody, status);
        }

        private ExecutorResponse GetExecutorResponse(HttpExecutorRequest request, DateTime startTimeUtc, string responseBody, string status, AttemptSummary attemptSummary)
        {
            DateTime endTimeUtc = System.DateTime.Now.ToUniversalTime();
            attemptSummary.AttemptCount = 1;
            attemptSummary.Attempts.Add(new Attempt(1, status, startTimeUtc, endTimeUtc));
            var results = JsonSerializer.Deserialize<dynamic>(responseBody);
            return new ExecutorResponse(request.CorrelationId, status, startTimeUtc, endTimeUtc, attemptSummary, results);
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

        private Polly.Wrap.AsyncPolicyWrap<HttpResponseMessage> SetResiliencePolicy(HttpExecutorRequest request)
        {
            AsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(request.ExecutionPolicy.MaxRetries, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromSeconds(new Random().Next(0, request.ExecutionPolicy.DelayTimeoutInSeconds))
            );


            AsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: request.ExecutionPolicy.BreakTimeoutInSeconds,
                    durationOfBreak: TimeSpan.FromSeconds(request.ExecutionPolicy.BreakTimeoutInSeconds)
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
            return string.Join("&", encodedPairs);
        }
    }
}
