using FluentAssertions;
using FluentValidation;
using NSubstitute;
using RemoteExecutorApi.API.Validators;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Enums;
using RemoteExecutorGateWayApi.Services;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;
using System.Text.Json;

namespace RemoteExecutorGateWayApi.UnitTests.Services
{
    public class OrchestratorServiceTests
    {
        private readonly IHttpExecutorService httpExecutorService = Substitute.For<IHttpExecutorService>();
        private readonly IPowershellExecutorService powershellExecutorService = Substitute.For<IPowershellExecutorService>();
        private readonly ExecutorJsonRequestValidator validator = new ExecutorJsonRequestValidator();

        private OrchestratorService service;

        public OrchestratorServiceTests()
        {
            service = new OrchestratorService(httpExecutorService, powershellExecutorService, validator);
        }
        [Fact]
        public async Task ExecuteAsync_InputParameterIsNull_ThrowArgumentExeception()
        {
            //Arrange
            ExecutorJsonRequest executorJson = null;

            //Act
            Func<Task> act = async () => await service.ExecuteAsync(executorJson);
            ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(act);

            //Assert
            exception.Message.Should().Be("Value cannot be null. (Parameter 'executorJson')");

        }

        [Fact]
        public async Task ExecuteAsync_AnyValidatorRuleFails_ThrowValidationException()
        {
            //Arrange
            var policy = @"{ ""maxRetries"": ""3"" }";
            var body = @"{ ""method"": ""Post"" }";

            ExecutorJsonRequest executorJson = new()
            {
                ExecutorType = 3,
                Policy = GetJsonElement(policy),
                RequestBody = GetJsonElement(body)
            };

            //Act
            Func<Task> act = async () => await service.ExecuteAsync(executorJson);
            ValidationException exception = await Assert.ThrowsAsync<ValidationException>(act);

            //Assert
            exception.Message.Should().Contain("ExecutionType is not valid");

        }

        [Fact]
        public async Task ExecuteAsync_HttpRequest_CallHttpRequestExecutor()
        {
            //Arrange
            var policy = @"{ ""maxRetries"": 3 }";
            var body = @"{ ""method"": ""Post"" }";

            ExecutorJsonRequest executorJson = new()
            {
                ExecutorType = (int)ExecutionTypeEnum.Http,
                Policy = GetJsonElement(policy),
                RequestBody = GetJsonElement(body)
            };

            httpExecutorService.ExecuteAsync(Arg.Any<HttpExecutorRequest>()).Returns(Task.FromResult(new ExecutorResponse { Status = "success" }));

            //Act
            var result = await service.ExecuteAsync(executorJson);

            //Assert
            await httpExecutorService.Received(1).ExecuteAsync(Arg.Any<HttpExecutorRequest>());
            result.Status.Should().Be("success");
        }

        [Fact]
        public async Task ExecuteAsync_PowershellRequest_CallPowerShellExecutorRequestExecutor()
        {
            //Arrange
            var policy = @"{ ""maxRetries"": 3 }";
            var body = @"{ ""method"": ""Post"" }";

            ExecutorJsonRequest executorJson = new()
            {
                ExecutorType = (int)ExecutionTypeEnum.PowerShell,
                Policy = GetJsonElement(policy),
                RequestBody = GetJsonElement(body)
            };

            powershellExecutorService.ExecuteAsync(Arg.Any<PowerShellExecutorRequest>()).Returns(Task.FromResult(new ExecutorResponse { Status = "success" }));

            //Act
            var result = await service.ExecuteAsync(executorJson);

            //Assert
            await powershellExecutorService.Received(1).ExecuteAsync(Arg.Any<PowerShellExecutorRequest>());
            result.Status.Should().Be("success");
        }

        private JsonElement GetJsonElement(string jsonString)
        {
            JsonDocument document = JsonDocument.Parse(jsonString);
            return document.RootElement;
        }
    }
}