using Shouldly;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class StartLoanShould
{
    [Fact]
    public void Fail_if_book_copy_is_already_loaned()
    {
        // HW
    }

    [Fact]
    public void Fail_if_user_does_not_have_a_reservation_for_book_copy()
    {
        // HW
    }

    [Fact]
    public void Remove_existing_reservation()
    {
        // HW
    }

    [Fact]
    public void Start_new_loan_and_publish_BookLoanedEvent()
    {
        // HW
    }
}