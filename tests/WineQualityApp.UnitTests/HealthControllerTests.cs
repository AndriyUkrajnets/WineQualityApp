using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WineQualityApp.Api.Controllers;

namespace WineQualityApp.UnitTests;

public sealed class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsHealthyStatus()
    {
        var result = new HealthController().Get();

        var response = Assert.IsType<OkObjectResult>(result);
        var body = JsonSerializer.SerializeToElement(response.Value);
        Assert.Equal("healthy", body.GetProperty("status").GetString());
    }
}
