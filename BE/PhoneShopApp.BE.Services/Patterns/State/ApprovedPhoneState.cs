using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Services.Patterns.State;

public class ApprovedPhoneState : IPhoneReviewState
{
    public PhoneStatus Status => PhoneStatus.Approved;

    public void Approve(Phone phone)
    {
        throw new InvalidOperationException("Phone is already approved.");
    }

    public void Reject(Phone phone)
    {
        throw new InvalidOperationException("Approved phone cannot be rejected directly.");
    }
}
