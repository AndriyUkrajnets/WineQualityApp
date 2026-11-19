using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using WineQualityApp.Application.Abstractions;
using WineQualityApp.Domain;

namespace WineQualityApp.Infrastructure;

internal sealed class PythonMlClient(
    HttpClient httpClient,
    ILogger<PythonMlClient> logger) : IPythonPredictionClient
{
    public async Task<double> PredictAsync(
        WineCharacteristics wine,
        CancellationToken cancellationToken = default)
    {
        using var response = await SendPredictionAsync(wine, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Python prediction service returned HTTP {StatusCode}",
                (int)response.StatusCode);

            throw new PredictionProviderException(
                "The Python prediction service returned an error.",
                (int)response.StatusCode);
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<PythonPredictionResponse>(
                cancellationToken: cancellationToken);

            if (result is null || !double.IsFinite(result.PredictedQuality))
            {
                throw new JsonException("The Python service returned no finite prediction.");
            }

            return result.PredictedQuality;
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Python prediction service returned an invalid response");
            throw new PredictionProviderException(
                "The Python prediction service returned an invalid response.",
                (int)HttpStatusCode.BadGateway,
                exception);
        }
    }

    private async Task<HttpResponseMessage> SendPredictionAsync(
        WineCharacteristics wine,
        CancellationToken cancellationToken)
    {
        var request = PythonPredictionRequest.From(wine);

        try
        {
            return await httpClient.PostAsJsonAsync("predict", request, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "Could not reach the Python prediction service");
            throw new PredictionProviderException(
                "The Python prediction service is unavailable.",
                (int)HttpStatusCode.ServiceUnavailable,
                exception);
        }
        catch (OperationCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Python prediction service request timed out");
            throw new PredictionProviderException(
                "The Python prediction service timed out.",
                (int)HttpStatusCode.ServiceUnavailable,
                exception);
        }
    }

    private sealed record PythonPredictionRequest(
        [property: JsonPropertyName("fixed_acidity")] double FixedAcidity,
        [property: JsonPropertyName("volatile_acidity")] double VolatileAcidity,
        [property: JsonPropertyName("citric_acid")] double CitricAcid,
        [property: JsonPropertyName("residual_sugar")] double ResidualSugar,
        [property: JsonPropertyName("chlorides")] double Chlorides,
        [property: JsonPropertyName("free_sulfur_dioxide")] double FreeSulfurDioxide,
        [property: JsonPropertyName("total_sulfur_dioxide")] double TotalSulfurDioxide,
        [property: JsonPropertyName("density")] double Density,
        [property: JsonPropertyName("pH")] double PH,
        [property: JsonPropertyName("sulphates")] double Sulphates,
        [property: JsonPropertyName("alcohol")] double Alcohol,
        [property: JsonPropertyName("color")] string Color)
    {
        public static PythonPredictionRequest From(WineCharacteristics wine) => new(
            wine.FixedAcidity,
            wine.VolatileAcidity,
            wine.CitricAcid,
            wine.ResidualSugar,
            wine.Chlorides,
            wine.FreeSulfurDioxide,
            wine.TotalSulfurDioxide,
            wine.Density,
            wine.PH,
            wine.Sulphates,
            wine.Alcohol,
            wine.Color);
    }

    private sealed record PythonPredictionResponse(
        [property: JsonPropertyName("predicted_quality")] double PredictedQuality);
}
