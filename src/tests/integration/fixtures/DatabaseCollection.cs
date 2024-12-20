using System;

namespace tests.integration.fixtures;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseTestContext>
{
}
