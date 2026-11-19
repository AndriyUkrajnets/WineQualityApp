using WineQualityApp.Application.Abstractions;
using WineQualityApp.Application.Contracts;
using WineQualityApp.Domain;

namespace WineQualityApp.Application.Services;

public sealed class PredictionService(IPythonPredictionClient pythonPredictionClient)
    : IPredictionService
{
    public async Task<PredictionResponse> PredictAsync(
        CreatePredictionRequest request,
        CancellationToken cancellationToken = default)
    {
        var wine = new WineCharacteristics(
            request.FixedAcidity!.Value,
            request.VolatileAcidity!.Value,
            request.CitricAcid!.Value,
            request.ResidualSugar!.Value,
            request.Chlorides!.Value,
            request.FreeSulfurDioxide!.Value,
            request.TotalSulfurDioxide!.Value,
            request.Density!.Value,
            request.PH!.Value,
            request.Sulphates!.Value,
            request.Alcohol!.Value,
            request.Color!);

        var predictedQuality = await pythonPredictionClient.PredictAsync(wine, cancellationToken);
        return new PredictionResponse(predictedQuality);
    }
}
