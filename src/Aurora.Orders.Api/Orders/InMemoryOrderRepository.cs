using System.Collections.Concurrent;

namespace Aurora.Orders.Api.Orders;

/// <summary>Stand-in for the database, so the sample runs without one.</summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Order> _orders = new();

    public Task<Order?> FindAsync(string orderId, CancellationToken cancellationToken) =>
        Task.FromResult(_orders.TryGetValue(orderId, out Order? order) ? order : null);

    public Task SaveAsync(Order order, CancellationToken cancellationToken)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
