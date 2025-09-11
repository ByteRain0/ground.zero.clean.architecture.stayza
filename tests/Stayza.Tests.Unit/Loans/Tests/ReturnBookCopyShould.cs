using Shouldly;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class ReturnBookCopyShould
{
    [Fact]
    public void Fail_if_copy_is_not_loaned()
    {
        // HW
    }

    [Fact]
    public void Fail_if_book_copy_was_loaned_by_a_different_user()
    {
        // HW
    }

    [Fact]
    public void Mark_current_loan_as_returned_and_publish_BookReturnedEvent()
    {
        // HW
    }
}