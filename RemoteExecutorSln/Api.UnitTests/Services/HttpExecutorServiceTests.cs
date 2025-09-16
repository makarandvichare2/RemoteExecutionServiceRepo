using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using RemoteExecutorGateWayApi.Services;
using RemoteExecutorGateWayApi.UnitTests.Helpers;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using System.Net;
using System.Text.Json;

namespace RemoteExecutorGateWayApi.UnitTests.Services
{
    public class HttpExecutorServiceTests
    {
        private readonly MockHttpMessageHandler mockHttpHandler;
        private readonly HttpExecutorService service;
        private readonly HttpClient httpClient;
        private readonly IValidator<HttpExecutorRequest> validator;
        public HttpExecutorServiceTests()
        {
            validator = Substitute.For<IValidator<HttpExecutorRequest>>();
            mockHttpHandler = new MockHttpMessageHandler();
            httpClient = new HttpClient(mockHttpHandler);
            var mockFactory = Substitute.For<IHttpClientFactory>();
            mockFactory.CreateClient().Returns(httpClient);
            service = new HttpExecutorService(validator, mockFactory);
        }
        [Fact]
        public async Task ExecuteAsync_InputParameterIsNull_ThrowArgumentExeception()
        {
            //Arrange
            HttpExecutorRequest request = null;

            //Act
            Func<Task> act = async () => await service.ExecuteAsync(request);
            ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(act);

            //Assert
            exception.Message.Should().Be("Value cannot be null. (Parameter 'request')");

        }

        [Fact]
        public async Task ExecuteAsync_AnyValidatorRuleFails_ThrowValidationException()
        {
            //Arrange
            var request = CreateRequest();
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Url", "Url is required.") });
            validator.ValidateAsync(request).Returns(validationResult);

            //Act
            Func<Task> act = async () => await service.ExecuteAsync(request);
            ValidationException exception = await Assert.ThrowsAsync<ValidationException>(act);

            //Assert
            exception.Message.Should().Contain("Url is required.");
        }

        [Fact]
        public async Task ExecuteAsync_RequestIsSuccessful_ReturnsSuccessStatus()
        {
            // Arrange
            var request = CreateRequest();
            validator.ValidateAsync(request).Returns(new ValidationResult());
            mockHttpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"message\":\"success\"}")
            };

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.Status.Should().Be("sucess");
            ((JsonElement)response.Result).GetProperty("message").GetString().Should().Be("success");
        }

        [Fact]
        public async Task ExecuteAsync_RequestSucceedsOnFirstAttempt_AttemptSummaryHasOneAttempt()
        {
            // Arrange
            var request = CreateRequest();
            validator.ValidateAsync(request).Returns(new ValidationResult());
            mockHttpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"message\":\"success\"}")
            };
            mockHttpHandler.ResetCallCount();

            // Act
            await service.ExecuteAsync(request);

            // Assert
            mockHttpHandler.CallCount.Should().Be(1);
        }

        [Fact]
        public async Task ExecuteAsync_TransientFailure_SucceedsAfterRetry()
        {
            // Arrange
            var request = CreateRequest(maxRetries: 2);
            validator.ValidateAsync(request).Returns(new ValidationResult());
            mockHttpHandler.ResponseQueue.Enqueue(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            mockHttpHandler.ResponseQueue.Enqueue(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            mockHttpHandler.ResponseQueue.Enqueue(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.Status.Should().Be("sucess");
            mockHttpHandler.CallCount.Should().Be(3);
        }

        [Fact]
        public async Task ExecuteAsync_BreaksCircuit_ReturnsBrokenCircuitMessage()
        {
            // Arrange
            var request = CreateRequest(maxEventsBeforeBreak: 2);
            validator.ValidateAsync(request).Returns(new ValidationResult());
            mockHttpHandler.ResponseQueue.Enqueue(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            mockHttpHandler.ResponseQueue.Enqueue(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)); // This will break the circuit

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.Status.Should().Be("failed");
            ((JsonElement)response.Result).GetProperty("errorMessage").GetString().Should().Contain("Circuit breaker is open");
            mockHttpHandler.CallCount.Should().Be(2);
        }

        private HttpExecutorRequest CreateRequest(int maxRetries =3, int maxEventsBeforeBreak =3)
        {
            HttpRequestBody body = new()
            {
                Method = "POST",
                Url="http://localtest"
            };
            ExecutorRequestPolicy policy = new()
            {
                MaxRetries = maxRetries,
                MaxEventBeforeBreak = maxEventsBeforeBreak
            };
            return new HttpExecutorRequest(body, policy);
        }
    }
}