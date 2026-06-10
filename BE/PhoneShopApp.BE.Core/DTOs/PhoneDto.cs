using PhoneShopApp.BE.Core.Enums;

namespace PhoneShopApp.BE.Core.DTOs;

public class PhoneDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Specifications { get; set; } = string.Empty;
    public PhoneStatus Status { get; set; }
}
