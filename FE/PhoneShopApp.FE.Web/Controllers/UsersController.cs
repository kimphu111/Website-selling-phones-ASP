using PhoneShopApp.FE.Web.Models.Users;
using PhoneShopApp.FE.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace PhoneShopApp.FE.Web.Controllers;

public class UsersController : Controller
{
    private readonly BackendApiClient _backendApiClient;

    public UsersController(BackendApiClient backendApiClient)
    {
        _backendApiClient = backendApiClient;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        var users = await _backendApiClient.GetUsersAsync(token, cancellationToken);
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model, CancellationToken cancellationToken)
    {
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        if (ModelState.IsValid)
        {
            await _backendApiClient.CreateUserAsync(model, token, cancellationToken);
        }
        return RedirectToAction(nameof(Index));
    }
}
