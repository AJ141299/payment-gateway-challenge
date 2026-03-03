using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Repositories;
using PaymentGateway.Infrastructure.Clients;
using PaymentGateway.Infrastructure.Repositories;
using PaymentGateway.IntegrationTests.TestHelpers;

namespace PaymentGateway.IntegrationTests.Hooks;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public PaymentsRepository PaymentsRepository { get; } = new();
    public FakeBankHttpHandler BankHandler { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // replace with our controllable instances
            services.AddSingleton<IPaymentsRepository>(PaymentsRepository);
            services.AddHttpClient<IBankClient, BankClient>().ConfigurePrimaryHttpMessageHandler(() => BankHandler);
        });
    }
}