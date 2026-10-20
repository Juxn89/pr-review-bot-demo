using Aurora.Orders.Api.Payments;

namespace Aurora.Orders.Api.Orders;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id}", async (string id, OrderService orders, CancellationToken ct) =>
            await orders.GetAsync(id, ct) is { } order ? Results.Ok(order) : Results.NotFound());

        app.MapPost("/orders/{id}/pay", async (string id, string cardToken, OrderService orders, PaymentsClient payments, CancellationToken ct) =>
        {
            Order? order = await orders.GetAsync(id, ct);
            if (order is null)
            {
                return Results.NotFound();
            }

            PaymentResult result = await payments.ChargeAsync(new ChargeRequest(order.Id, order.Total, "USD", cardToken), ct);
            return result.Approved ? Results.Ok(await orders.MarkPaidAsync(order, ct)) : Results.UnprocessableEntity();
        });
    }
}
