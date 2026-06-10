using PhoneShopApp.FE.Web.Models.Phones;
using PhoneShopApp.FE.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace PhoneShopApp.FE.Web.Controllers;

public class PhonesController : Controller
{
    private readonly BackendApiClient _backendApiClient;

    public PhonesController(BackendApiClient backendApiClient)
    {
        _backendApiClient = backendApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword, string? searchBy, CancellationToken cancellationToken)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var phones = await _backendApiClient.SearchPhonesAsync(keyword, searchBy, token, cancellationToken);
        ViewData["Keyword"] = keyword;
        ViewData["SearchBy"] = searchBy;
        ViewData["IsAdmin"] = HttpContext.Session.GetString("role") == "Admin";
        return View(phones);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePhoneViewModel phone, CancellationToken cancellationToken)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        await _backendApiClient.CreatePhoneAsync(phone, token, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, CreatePhoneViewModel phone, CancellationToken cancellationToken)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        await _backendApiClient.UpdatePhoneAsync(Id, phone, token, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, CancellationToken cancellationToken)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        await _backendApiClient.UpdatePhoneStatusAsync(id, status, token, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Login", "Auth");

        await _backendApiClient.DeletePhoneAsync(id, token, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private string? GetToken()
    {
        return HttpContext.Session.GetString("jwt_token");
    }
}
