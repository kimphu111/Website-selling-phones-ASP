namespace PhoneShopApp.FE.Web.Models.Auth;

public class LoginResponseViewModel
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
