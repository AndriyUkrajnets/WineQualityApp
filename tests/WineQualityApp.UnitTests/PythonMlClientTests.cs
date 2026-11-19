using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using WineQualityApp.Application.Abstractions;
using WineQualityApp.Infrastructure;

namespace WineQualityApp.UnitTests;

public sealed class PythonMlClientTests
{
    [Fact]
    public async Task PredictAsync_PostsPythonContractAndReadsPrediction()
    {
        string? requestPath = null;
        string? requestBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            requestPath = request.RequestUri?.AbsolutePath;
            requestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            return JsonResponse(HttpStatusCode.OK, """{"predicted_quality":5.1675}""");
        });
        var client = CreateClient(handler);

        var prediction = await client.PredictAsync(WineTestData.CreateCharacteristics());

        Assert.Equal(5.1675, prediction);
        Assert.Equal("/predict", requestPath);
        using var body = JsonDocument.Parse(requestBody!);
        Assert.Equal(7.4, body.RootElement.GetProperty("fixed_acidity").GetDouble());
        Assert.Equal(3.51, body.RootElement.GetProperty("pH").GetDouble());
        Assert.Equal("red", body.RootElement.GetProperty("color").GetString());
    }

    [Fact]
    public async Task PredictAsync_MapsPythonUnavailableToServiceUnavailable()
    {
        var client = CreateClient(new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.ServiceUnavailable, "{}"))));

        var exception = await Assert.ThrowsAsync<PredictionProviderException>(
            () => client.PredictAsync(WineTestData.CreateCharacteristics()));

        Assert.Equal((int)HttpStatusCode.ServiceUnavailable, exception.ProviderStatusCode);
    }

    [Fact]
    public async Task PredictAsync_MapsNetworkFailureToServiceUnavailable()
    {
        var client = CreateClient(new StubHttpMessageHandler((_, _) =>
            throw new HttpRequestException("connection refused")));

        var exception = await Assert.ThrowsAsync<PredictionProviderException>(
            () => client.PredictAsync(WineTestData.CreateCharacteristics()));

        Assert.Equal((int)HttpStatusCode.ServiceUnavailable, exception.ProviderStatusCode);
    }

    [Fact]
    public async Task PredictAsync_MapsInvalidPythonResponseToBadGateway()
    {
        var client = CreateClient(new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "not-json"))));

        var exception = await Assert.ThrowsAsync<PredictionProviderException>(
            () => client.PredictAsync(WineTestData.CreateCharacteristics()));

        Assert.Equal((int)HttpStatusCode.BadGateway, exception.ProviderStatusCode);
    }

    private static PythonMlClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://python-service/")
        };

        return new PythonMlClient(httpClient, NullLogger<PythonMlClient>.Instance);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json) =>
        new(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => send(request, cancellationToken);
    }
}
