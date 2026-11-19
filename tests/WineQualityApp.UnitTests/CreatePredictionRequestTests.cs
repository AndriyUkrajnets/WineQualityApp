using System.ComponentModel.DataAnnotations;
using WineQualityApp.Application.Contracts;

namespace WineQualityApp.UnitTests;

public sealed class CreatePredictionRequestTests
{
    [Fact]
    public void Validate_AcceptsCompleteRequestWithSupportedColor()
    {
        var request = WineTestData.CreateRequest();

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("pink")]
    [InlineData("Red")]
    [InlineData("")]
    public void Validate_RejectsUnsupportedColor(string color)
    {
        var request = WineTestData.CreateRequest(color: color);

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Color)));
    }

    [Fact]
    public void Validate_RejectsMissingFeature()
    {
        var request = WineTestData.CreateRequest(alcohol: null);

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(request.Alcohol)));
    }

    private static List<ValidationResult> Validate(CreatePredictionRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }
}
