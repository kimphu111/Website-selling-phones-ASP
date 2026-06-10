using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhoneShopApp.BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PhonesController : ControllerBase
{
    private readonly IPhoneShopFacade _phoneShopFacade;

    public PhonesController(IPhoneShopFacade phoneShopFacade)
    {
        _phoneShopFacade = phoneShopFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<PhoneDto>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] string? searchBy,
        CancellationToken cancellationToken)
    {
        var phones = await _phoneShopFacade.SearchPhonesAsync(keyword, searchBy, cancellationToken);
        return Ok(phones);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PhoneDto>> Create([FromBody] CreatePhoneDto dto, CancellationToken cancellationToken)
    {
        var created = await _phoneShopFacade.CreatePhoneAsync(dto, cancellationToken);
        return Ok(created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PhoneDto>> Update(int id, [FromBody] UpdatePhoneDto dto, CancellationToken cancellationToken)
    {
        var updated = await _phoneShopFacade.UpdatePhoneAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PhoneDto>> UpdateStatus(int id, [FromBody] UpdatePhoneStatusDto dto, CancellationToken cancellationToken)
    {
        var updated = await _phoneShopFacade.UpdatePhoneStatusAsync(id, dto.Action, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _phoneShopFacade.DeletePhoneAsync(id, cancellationToken);
        return NoContent();
    }
}
