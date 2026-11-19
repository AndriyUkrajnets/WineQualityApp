using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using WineQualityApp.Api.Controllers;
using WineQualityApp.Application.Abstractions;
using WineQualityApp.Application.Contracts;
using WineQualityApp.Application.Services;

namespace WineQualityApp.UnitTests;

public sealed class PredictionsControllerTests
{
    [Fact]
    public async Task Create_ReturnsPredictionFromApplicationService()
    {
        var expected = new PredictionResponse(5.1675);
        var controller = CreateController(new StubPredictionService(expected));

        var action = await controller.Create(WineTestData.CreateRequest(), CancellationToken.None);

        var result = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Equal(expected, Assert.IsType<PredictionResponse>(result.Value));
    }

    [Theory]
    [InlineData(503, 503)]
    [InlineData(502, 502)]
    [InlineData(422, 502)]
    public async Task Create_ReturnsProblemDetailsForProviderFailure(
        int providerStatusCode,
        int expectedStatusCode)
    {
        var controller = CreateController(new StubPredictionService(
            new PredictionProviderException("provider failure", providerStatusCode)));

        var action = await controller.Create(WineTestData.CreateRequest(), CancellationToken.None);

        var result = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(expectedStatusCode, result.StatusCode);
        Assert.IsType<ProblemDetails>(result.Value);
    }

    private static PredictionsController CreateController(IPredictionService service) =>
        new(service, NullLogger<PredictionsController>.Instance);

    private sealed class StubPredictionService : IPredictionService
    {
        private readonly PredictionResponse _response;
        private readonly Exception? _failure;

        public StubPredictionService(PredictionResponse response)
        {
            _response = response;
        }

        public StubPredictionService(Exception failure)
        {
            _response = new PredictionResponse(5.1675);
            _failure = failure;
        }

        public Task<PredictionResponse> PredictAsync(
            CreatePredictionRequest request,
            CancellationToken cancellationToken = default) =>
            _failure is null
                ? Task.FromResult(_response)
                : Task.FromException<PredictionResponse>(_failure);
    }
}
