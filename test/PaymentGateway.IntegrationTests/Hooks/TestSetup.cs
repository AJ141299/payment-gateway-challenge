using Reqnroll;

namespace PaymentGateway.IntegrationTests.Hooks;

[Binding]
public class TestSetup
{
    private readonly ScenarioContext _scenarioContext;

    public TestSetup(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        _scenarioContext["Factory"] = factory;
        _scenarioContext["HttpClient"] = client;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        if (_scenarioContext.TryGetValue("Factory", out TestWebApplicationFactory factory))
            factory.Dispose();
    }
}