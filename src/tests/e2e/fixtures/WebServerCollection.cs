using System;

namespace tests.e2e.fixtures;

[CollectionDefinition("Web server collection")]
public class WebServerCollection : ICollectionFixture<WebServer>
{
}
