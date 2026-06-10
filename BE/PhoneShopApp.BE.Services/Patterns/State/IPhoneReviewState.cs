using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Services.Patterns.State;

public interface IPhoneReviewState
{
    PhoneStatus Status { get; }
    void Approve(Phone phone);
    void Reject(Phone phone);
}
