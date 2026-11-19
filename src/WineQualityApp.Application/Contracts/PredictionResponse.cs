using System.Text.Json.Serialization;

namespace WineQualityApp.Application.Contracts;

public sealed record PredictionResponse(
    [property: JsonPropertyName("predicted_quality")] double PredictedQuality);
