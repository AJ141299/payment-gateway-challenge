using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using FluentValidation;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;
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
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<PaymentsRepository>();
        builder.AddClients();
        builder.AddServices();
        builder.AddRepositories();
        builder.AddValidators();
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
    
    private static void AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IValidator<ProcessPaymentRequest>, ProcessPaymentRequestValidator>();
    }
}