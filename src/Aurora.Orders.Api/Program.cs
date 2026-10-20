using Aurora.Orders.Api.Orders;
using Aurora.Orders.Api.Payments;

// Aurora Orders: a small orders API used to demonstrate claude-pr-review-bot.
// It is complete enough to build and to read; the pull requests against it are the interesting part.

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<OrderService>();

// The payments processor authenticates with an API key from configuration (user secrets locally,
// a key vault in production). It must never be logged.
builder.Services.AddHttpClient<PaymentsClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Payments:BaseUrl"]!);
})
.AddStandardResilienceHandler();

WebApplication app = builder.Build();
app.MapOrderEndpoints();

app.Run();
