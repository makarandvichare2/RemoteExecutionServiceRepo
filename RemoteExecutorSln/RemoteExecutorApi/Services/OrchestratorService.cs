using RemoteExecutorGateWayApi.Commands;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Enums;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;
using System.Text.Json;
namespace RemoteExecutorGateWayApi.Services
{
    public class OrchestratorService
    {
        private readonly IHttpExecutorService httpExecutorService;
        private readonly IPowershellExecutorService powershellExecutorService;

        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public OrchestratorService(
            IHttpExecutorService httpExecutorService,
            IPowershellExecutorService powershellExecutorService) {
             this.httpExecutorService = httpExecutorService;
             this.powershellExecutorService = powershellExecutorService;
        }
        public async Task<ExecutorResponse> ExecuteAsync(ExecutorJsonRequest executorJson)
        {
            // The ExecutionRequest object is already populated by the controller's model binding

            if (executorJson == null) throw new ArgumentNullException(nameof(executorJson));

            if (executorJson.ExecutorType == (int)ExecutionTypeEnum.Http)
            {
                return await httpExecutorService.ExecuteAsync(new HttpExecutorRequest(
                    CreateHttpRequestBody(executorJson.RequestBody),
                    CreatePolicy(executorJson.Policy)));
            }

            if (executorJson.ExecutorType == (int)ExecutionTypeEnum.PowerShell)
            {
                return await powershellExecutorService.ExecuteAsync(new PowerShellExecutorRequest(
                    CreatePowershellRequestBody(executorJson.RequestBody),
                    CreatePolicy(executorJson.RequestBody)));
            }

            return null;

        }

        private HttpRequestBody CreateHttpRequestBody(JsonElement requestBody)
        {
            return JsonSerializer.Deserialize<HttpRequestBody>(requestBody.GetRawText(), options);
        }

        private PowerShellRequestBody CreatePowershellRequestBody(JsonElement requestBody)
        {
            return JsonSerializer.Deserialize<PowerShellRequestBody>(requestBody.GetRawText());
        }

        private ExecutorRequestPolicy CreatePolicy(JsonElement policyJson)
        {
            var policy = JsonSerializer.Deserialize<ExecutorRequestPolicy>(policyJson.GetRawText(), options);
            return new ExecutorRequestPolicy
            {
                MaxRetries = policy.MaxRetries,
                TimeoutInSeconds = policy.TimeoutInSeconds,
            };
        }

    }
}
