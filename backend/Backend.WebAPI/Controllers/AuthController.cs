using Backend.Application.Commands.SignIn;
using Backend.Application.Commands.SignUp;
using Backend.Domain.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    ICommandDispatcher commandDispatcher
    ) : ControllerBase
{
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInCommand cmd)
    {
        var result = await commandDispatcher.DispatchAsync<SignInCommand, string>(cmd);
        return Ok(result);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpCommand cmd)
    {
        var result = await commandDispatcher.DispatchAsync<SignUpCommand, string>(cmd);
        return Ok(result);
    }
}