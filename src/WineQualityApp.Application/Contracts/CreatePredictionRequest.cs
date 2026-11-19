using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WineQualityApp.Application.Contracts;

public sealed class CreatePredictionRequest
{
    [Required, JsonPropertyName("fixed_acidity")]
    public double? FixedAcidity { get; init; }

    [Required, JsonPropertyName("volatile_acidity")]
    public double? VolatileAcidity { get; init; }

    [Required, JsonPropertyName("citric_acid")]
    public double? CitricAcid { get; init; }

    [Required, JsonPropertyName("residual_sugar")]
    public double? ResidualSugar { get; init; }

    [Required, JsonPropertyName("chlorides")]
    public double? Chlorides { get; init; }

    [Required, JsonPropertyName("free_sulfur_dioxide")]
    public double? FreeSulfurDioxide { get; init; }

    [Required, JsonPropertyName("total_sulfur_dioxide")]
    public double? TotalSulfurDioxide { get; init; }

    [Required, JsonPropertyName("density")]
    public double? Density { get; init; }

    [Required, JsonPropertyName("pH")]
    public double? PH { get; init; }

    [Required, JsonPropertyName("sulphates")]
    public double? Sulphates { get; init; }

    [Required, JsonPropertyName("alcohol")]
    public double? Alcohol { get; init; }

    [Required, RegularExpression("^(red|white)$"), JsonPropertyName("color")]
    public string? Color { get; init; }
}
