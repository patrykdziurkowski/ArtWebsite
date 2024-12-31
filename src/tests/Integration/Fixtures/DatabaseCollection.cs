namespace tests.integration.fixtures;

[CollectionDefinition("Database collection", DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<DatabaseTestContext>
{
}
