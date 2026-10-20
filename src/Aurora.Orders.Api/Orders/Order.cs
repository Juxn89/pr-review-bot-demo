namespace Aurora.Orders.Api.Orders;

public sealed record Order(string Id, string CustomerId, decimal Total, OrderStatus Status);

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled,
}
