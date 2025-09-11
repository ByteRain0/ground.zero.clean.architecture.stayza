using Shouldly;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class CancelReservationShould
{
    [Fact]
    public void Fail_if_reservation_does_not_exist()
    {
        // HW
    }

    [Fact]
    public void Fail_if_reservation_already_cancelled()
    {
        // HW
    }

    [Fact]
    public void Cancel_valid_reservation()
    {
        // HW
    }

    [Fact]
    public void Publish_ReservationCancelledEvent()
    {
        // HW
    }
}