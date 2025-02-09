namespace tests.E2E.Fixtures;

[CollectionDefinition("Web server collection", DisableParallelization = true)]
[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class WebServerCollection : ICollectionFixture<WebDriverInitializer>
{
}
