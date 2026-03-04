using PaymentGateway.Api;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureApp();
app.Run();
