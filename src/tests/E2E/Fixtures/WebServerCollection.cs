using System;

namespace tests.e2e.fixtures;

[CollectionDefinition("Web server collection", DisableParallelization = true)]
[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
public class WebServerCollection : ICollectionFixture<WebServer>
{
}
