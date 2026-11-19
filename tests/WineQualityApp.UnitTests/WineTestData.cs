using WineQualityApp.Application.Contracts;
using WineQualityApp.Domain;

namespace WineQualityApp.UnitTests;

internal static class WineTestData
{
    public static CreatePredictionRequest CreateRequest(
        string? color = "red",
        double? alcohol = 9.4) => new()
    {
        FixedAcidity = 7.4,
        VolatileAcidity = 0.7,
        CitricAcid = 0.0,
        ResidualSugar = 1.9,
        Chlorides = 0.076,
        FreeSulfurDioxide = 11.0,
        TotalSulfurDioxide = 34.0,
        Density = 0.9978,
        PH = 3.51,
        Sulphates = 0.56,
        Alcohol = alcohol,
        Color = color
    };

    public static WineCharacteristics CreateCharacteristics() => new(
        FixedAcidity: 7.4,
        VolatileAcidity: 0.7,
        CitricAcid: 0.0,
        ResidualSugar: 1.9,
        Chlorides: 0.076,
        FreeSulfurDioxide: 11.0,
        TotalSulfurDioxide: 34.0,
        Density: 0.9978,
        PH: 3.51,
        Sulphates: 0.56,
        Alcohol: 9.4,
        Color: "red");
}
