using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Services.Patterns.State;

public class PendingPhoneState : IPhoneReviewState
{
    public PhoneStatus Status => PhoneStatus.Pending;

    public void Approve(Phone phone)
    {
        phone.Status = PhoneStatus.Approved;
    }

    public void Reject(Phone phone)
    {
        phone.Status = PhoneStatus.Rejected;
    }
}
