using WineQualityApp.Application.Abstractions;
using WineQualityApp.Application.Services;
using WineQualityApp.Domain;

namespace WineQualityApp.UnitTests;

public sealed class PredictionServiceTests
{
    [Fact]
    public async Task PredictAsync_MapsRequestAndReturnsPythonPrediction()
    {
        var pythonClient = new RecordingPythonPredictionClient(5.1675);
        var service = new PredictionService(pythonClient);

        var response = await service.PredictAsync(WineTestData.CreateRequest());

        Assert.Equal(5.1675, response.PredictedQuality);
        Assert.Equal(WineTestData.CreateCharacteristics(), pythonClient.Wine);
    }

    private sealed class RecordingPythonPredictionClient(double prediction) : IPythonPredictionClient
    {
        public WineCharacteristics? Wine { get; private set; }

        public Task<double> PredictAsync(
            WineCharacteristics wine,
            CancellationToken cancellationToken = default)
        {
            Wine = wine;
            return Task.FromResult(prediction);
        }
    }
}
