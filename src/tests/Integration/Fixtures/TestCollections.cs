namespace tests.Integration.Fixtures;

[CollectionDefinition("Database collection", DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<DatabaseTestContext>;
