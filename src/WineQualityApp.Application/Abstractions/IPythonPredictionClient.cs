using WineQualityApp.Domain;

namespace WineQualityApp.Application.Abstractions;

public interface IPythonPredictionClient
{
    Task<double> PredictAsync(
        WineCharacteristics wine,
        CancellationToken cancellationToken = default);
}
