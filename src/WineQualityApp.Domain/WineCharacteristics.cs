namespace WineQualityApp.Domain;

public sealed record WineCharacteristics(
    double FixedAcidity,
    double VolatileAcidity,
    double CitricAcid,
    double ResidualSugar,
    double Chlorides,
    double FreeSulfurDioxide,
    double TotalSulfurDioxide,
    double Density,
    double PH,
    double Sulphates,
    double Alcohol,
    string Color);
