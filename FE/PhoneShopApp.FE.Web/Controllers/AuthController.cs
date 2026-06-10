using PhoneShopApp.FE.Web.Models.Auth;
using PhoneShopApp.FE.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace PhoneShopApp.FE.Web.Controllers;

public class AuthController : Controller
{
    private readonly BackendApiClient _backendApiClient;

    public AuthController(BackendApiClient backendApiClient)
    {
        _backendApiClient = backendApiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginResult = await _backendApiClient.LoginAsync(model.Username, model.Password, cancellationToken);
        if (loginResult is null)
        {
            ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu.");
            return View(model);
        }

        HttpContext.Session.SetString("jwt_token", loginResult.Token);
        HttpContext.Session.SetString("role", loginResult.Role);
        HttpContext.Session.SetString("jwt_expiry", loginResult.ExpiresAtUtc.ToString("O"));

        return RedirectToAction("Index", "Phones");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
