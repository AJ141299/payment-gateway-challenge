using PaymentGateway.Api.Clients;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<PaymentsRepository>();
        builder.AddClients();
    }
    
    public static void ConfigureApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void AddClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IBankClient, BankClient>(c =>
        {
            c.BaseAddress = new Uri("http://localhost:8080");
            c.Timeout = TimeSpan.FromSeconds(30);
        });
    }
}