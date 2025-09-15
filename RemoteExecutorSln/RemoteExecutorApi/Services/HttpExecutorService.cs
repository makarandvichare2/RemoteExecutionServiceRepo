using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Services
{
    public class HttpExecutorService : IHttpExecutorService
    {

        AsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError() 
            .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests) 
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100))
            ); 


        AsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5, 
                durationOfBreak: TimeSpan.FromSeconds(30) 
            );
        private readonly IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;
        public HttpExecutorService()
        {
            _resiliencePolicy = Policy.WrapAsync(circuitBreakerPolicy, retryPolicy);
        }
        public async Task<ExecutorResponse> ExecuteAsync(HttpExecutorRequest request)
        {

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var executorResult = await _resiliencePolicy.ExecuteAsync(async () =>
                {

                    var result = await executor.ExecuteAsync(request);
                    return result; 
                });

                // Build the success response.
            }
            catch (BrokenCircuitException ex)
            {
                // This exception is thrown by Polly when the circuit is open.
                // Build a specific failure response for this scenario.
            }
            catch (Exception ex)
            {
                // Any other final exceptions after retries exhausted or a permanent failure.
            }

            var response = new ExecutorResponse(request.CorrelationId,)
        }
    }
}
