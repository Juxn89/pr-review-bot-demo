using Microsoft.Extensions.Logging;

namespace Aurora.Orders.Api.Orders;

/// <summary>
/// Reads and updates orders. Registered as a singleton: one instance serves every request.
/// </summary>
/// <remarks>
/// Status changes are logged with the order id only; customer data never goes to the logs.
/// </remarks>
public sealed class OrderService(IOrderRepository repository, ILogger<OrderService> logger)
{
    private readonly IOrderRepository _repository = repository;
    private readonly ILogger<OrderService> _logger = logger;
    private readonly Dictionary<string, Order> _cache = new();

    public async Task<Order?> GetAsync(string orderId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(orderId, out Order? cached))
        {
            return cached;
        }

        Order? order = await _repository.FindAsync(orderId, cancellationToken);
        if (order is not null)
        {
            _cache[orderId] = order;
        }

        return order;
    }

    public async Task<Order> MarkPaidAsync(Order order, CancellationToken cancellationToken)
    {
        Order paid = order with { Status = OrderStatus.Paid };
        await _repository.SaveAsync(paid, cancellationToken);
        _logger.LogInformation("Order {OrderId} marked as paid", paid.Id);
        return paid;
    }
}
