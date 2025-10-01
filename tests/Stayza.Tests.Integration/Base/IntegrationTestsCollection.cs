namespace Stayza.Tests.Integration.Base;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection;

[CollectionDefinition("LoansTests")]
public class LoansTestsCollection : ICollectionFixture<ApiFactory>;

[CollectionDefinition("BooksTests")]
public class BooksTestsCollection;