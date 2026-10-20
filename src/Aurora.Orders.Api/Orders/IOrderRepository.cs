namespace Aurora.Orders.Api.Orders;

public interface IOrderRepository
{
    Task<Order?> FindAsync(string orderId, CancellationToken cancellationToken);

    Task SaveAsync(Order order, CancellationToken cancellationToken);
}
