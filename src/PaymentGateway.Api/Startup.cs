using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Repositories;
using PaymentGateway.Core.Services;
using PaymentGateway.Infrastructure.Clients;
using PaymentGateway.Infrastructure.Repositories;

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
        builder.AddServices();
        builder.AddRepositories();
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
    
    private static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPaymentsService, PaymentsService>();
    }
    
    private static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
    }
}