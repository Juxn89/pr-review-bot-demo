using System.Net.Http.Json;

namespace Aurora.Orders.Api.Payments;

public sealed record ChargeRequest(string OrderId, decimal Amount, string Currency, string CardToken);

public sealed record PaymentResult(bool Approved, string? Reference)
{
    public static readonly PaymentResult Declined = new(false, null);
}

/// <summary>
/// Talks to the payments processor. A timeout or a 5xx is not a decline: the charge may have gone
/// through, so callers retry with the same idempotency key instead of telling the customer it failed.
/// </summary>
public sealed class PaymentsClient(HttpClient http, ILogger<PaymentsClient> logger)
{
    private readonly ILogger<PaymentsClient> _logger = logger;

    /// <summary>Looks up a previous charge by order, for reconciliation.</summary>
    public async Task<PaymentResult?> FindChargeAsync(string orderId, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await http.GetAsync($"charges/{orderId}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        PaymentResult? result = await response.Content.ReadFromJsonAsync<PaymentResult>(cancellationToken);
        _logger.LogInformation("Charge for order {OrderId} found: approved={Approved}", orderId, result?.Approved);
        return result;
    }

    /// <summary>Charges the card and returns the processor's decision.</summary>
    public async Task<PaymentResult> ChargeAsync(ChargeRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await http.PostAsJsonAsync("charges", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PaymentResult>(cancellationToken))!;
    }

}
