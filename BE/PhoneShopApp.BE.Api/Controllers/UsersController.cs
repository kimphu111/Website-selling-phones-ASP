using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhoneShopApp.BE.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IPhoneShopFacade _phoneShopFacade;

    public UsersController(IPhoneShopFacade phoneShopFacade)
    {
        _phoneShopFacade = phoneShopFacade;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _phoneShopFacade.GetUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var user = await _phoneShopFacade.CreateUserAsync(dto, cancellationToken);
        return Ok(user);
    }
}
