using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using RemoteExecutorApi.Controllers;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Enums;
using RemoteExecutorGateWayApi.Services;
using RemoteExecutorGateWayApi.ViewModels.Responses;
using System.Text.Json;

namespace RemoteExecutorGateWayApi.UnitTests.Controllers
{
    public class ExecutorControllerTests
    {
        private readonly ExecutorController controller;
        private readonly IOrchestratorService service;
        public ExecutorControllerTests()
        {
            service = Substitute.For<IOrchestratorService>();
            controller = new ExecutorController(service);
        }

        [Fact]
        public async Task RunAsync_WithValidInput_ReturnOkObjectResult()
        {
            //Arrange
            var policy = @"{ ""maxRetries"": 3 }";
            var body = @"{ ""method"": ""Post"" }";
            var correlationId = Guid.NewGuid();
            ExecutorJsonRequest executorJson = new()
            {
                ExecutorType = (int)ExecutionTypeEnum.Http,
                Policy = GetJsonElement(policy),
                RequestBody = GetJsonElement(body)
            };

            service.ExecuteAsync(executorJson).Returns(Task.FromResult(new ExecutorResponse { CorrelationId = correlationId, Status = "success" }));
            
            // Act
            var result = await controller.RunAsync(executorJson);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<OkObjectResult>(actionResult);
            var resultValue = actionResult.Value as ExecutorResponse;
            Assert.Equal(resultValue.Status, "success");
        }

        [Fact]
        public async Task RunAsync_ValidationRuleFailed_ReturnOkObjectResult()
        {
            //Arrange
            ExecutorJsonRequest executorJson = null;
            service.ExecuteAsync(executorJson).Returns<Task<ExecutorResponse>>(x => { throw new ArgumentNullException("executorJson"); });

            // Act

            Func<Task> act = async () => await controller.RunAsync(executorJson);
            ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
            
            // Assert
            exception.Message.Should().Contain("Value cannot be null. (Parameter 'executorJson')");
        }

        private JsonElement GetJsonElement(string jsonString)
        {
            JsonDocument document = JsonDocument.Parse(jsonString);
            return document.RootElement;
        }
    }


}
