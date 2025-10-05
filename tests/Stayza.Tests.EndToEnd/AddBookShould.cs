namespace Stayza.Tests.EndToEnd;

[Collection("E2E")]
public class AddBookShould
{
    private readonly TestBase _testBase;
    
    public AddBookShould(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Add_new_book_if_not_exists()
    {
        Console.WriteLine("This is an e2e Test");
    }
}