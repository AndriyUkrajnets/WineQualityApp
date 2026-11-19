namespace WineQualityApp.Application.Abstractions;

public sealed class PredictionProviderException : Exception
{
    public PredictionProviderException(
        string message,
        int? providerStatusCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ProviderStatusCode = providerStatusCode;
    }

    public int? ProviderStatusCode { get; }
}
