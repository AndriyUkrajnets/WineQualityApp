using Microsoft.AspNetCore.Mvc;
using WineQualityApp.Application.Abstractions;
using WineQualityApp.Application.Contracts;
using WineQualityApp.Application.Services;

namespace WineQualityApp.Api.Controllers;

[ApiController]
[Route("api/predictions")]
public sealed class PredictionsController(
    IPredictionService predictionService,
    ILogger<PredictionsController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PredictionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<PredictionResponse>> Create(
        [FromBody] CreatePredictionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var prediction = await predictionService.PredictAsync(request, cancellationToken);
            return Ok(prediction);
        }
        catch (PredictionProviderException exception)
        {
            logger.LogWarning(
                exception,
                "Prediction request could not be completed by the Python service");

            var statusCode = exception.ProviderStatusCode == StatusCodes.Status503ServiceUnavailable
                ? StatusCodes.Status503ServiceUnavailable
                : StatusCodes.Status502BadGateway;
            var title = statusCode == StatusCodes.Status503ServiceUnavailable
                ? "Prediction service unavailable"
                : "Prediction service error";

            return Problem(
                statusCode: statusCode,
                title: title,
                detail: "The prediction could not be completed. Please try again later.");
        }
    }
}
