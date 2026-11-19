using WineQualityApp.Application.Contracts;

namespace WineQualityApp.Application.Services;

public interface IPredictionService
{
    Task<PredictionResponse> PredictAsync(
        CreatePredictionRequest request,
        CancellationToken cancellationToken = default);
}
