using Ardalis.Result;
using Ardalis.SharedKernel;
using MediatR;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Enums;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;
using System.Text.Json;

namespace RemoteExecutorGateWayApi.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public HttpRequestCommand Create(ExecutorJsonRequest executorJson)
        {
            if (executorJson == null) throw new ArgumentNullException(nameof(executorJson));

            if(executorJson.ExecutorType == (int)ExecutionTypeEnum.Http)
            {
                return new HttpRequestCommand(new HttpExecutorRequest(
                    CreateHttpRequestBody(executorJson.RequestBody),
                    CreatePolicy(executorJson.Policy)));
            }

            //if (executorJson.ExecutorType == (int)ExecutionTypeEnum.PowerShell)
            //{
            //    return new PowershellRequestCommand(new PowerShellExecutorRequest(
            //        CreatePowershellRequestBody(executorJson.RequestBody),
            //        CreatePolicy(executorJson.RequestBody)));
            //}

            return null;
        }

        private HttpRequestBody CreateHttpRequestBody(JsonElement requestBody) {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<HttpRequestBody>(requestBody.GetRawText(), options);

        }

        private PowerShellRequestBody CreatePowershellRequestBody(JsonElement requestBody)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<PowerShellRequestBody>(requestBody.GetRawText());
        }

        private ExecutorRequestPolicy CreatePolicy(JsonElement policyJson)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var policy = JsonSerializer.Deserialize<ExecutorRequestPolicy>(policyJson.GetRawText(), options);
            return new ExecutorRequestPolicy
            {
                MaxRetries = policy.MaxRetries,
                MaxEventBeforeBreak = policy.MaxEventBeforeBreak,
                DelayTimeoutInSeconds = policy.DelayTimeoutInSeconds,
                BreakTimeoutInSeconds = policy.BreakTimeoutInSeconds,
            };
        }

    }
}
