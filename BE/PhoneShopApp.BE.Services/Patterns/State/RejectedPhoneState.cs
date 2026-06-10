using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Services.Patterns.State;

public class RejectedPhoneState : IPhoneReviewState
{
    public PhoneStatus Status => PhoneStatus.Rejected;

    public void Approve(Phone phone)
    {
        throw new InvalidOperationException("Rejected phone cannot be approved directly.");
    }

    public void Reject(Phone phone)
    {
        throw new InvalidOperationException("Phone is already rejected.");
    }
}
