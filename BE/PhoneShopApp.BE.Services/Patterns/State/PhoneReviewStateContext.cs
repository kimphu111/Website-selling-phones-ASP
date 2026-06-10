using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Services.Patterns.State;

// State Pattern: quản lý chuyển trạng thái của Phone theo workflow Pending -> Approved/Rejected.
public class PhoneReviewStateContext
{
    private IPhoneReviewState _state;

    public PhoneReviewStateContext(PhoneStatus status)
    {
        _state = status switch
        {
            PhoneStatus.Pending => new PendingPhoneState(),
            PhoneStatus.Approved => new ApprovedPhoneState(),
            PhoneStatus.Rejected => new RejectedPhoneState(),
            _ => new PendingPhoneState()
        };
    }

    public void ApplyAction(Phone phone, string action)
    {
        if (string.Equals(action, "approve", StringComparison.OrdinalIgnoreCase))
        {
            _state.Approve(phone);
            _state = new ApprovedPhoneState();
            return;
        }

        if (string.Equals(action, "reject", StringComparison.OrdinalIgnoreCase))
        {
            _state.Reject(phone);
            _state = new RejectedPhoneState();
            return;
        }

        throw new InvalidOperationException("Action must be 'approve' or 'reject'.");
    }
}
