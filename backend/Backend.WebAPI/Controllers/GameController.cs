using System.Globalization;
using System.Security.Claims;
using Backend.Application.Commands.Games;
using Backend.Application.Commands.SignIn;
using Backend.Application.Commands.SignUp;
using Backend.Application.Common.Strings;
using Backend.Data.QueryHandlers.Games;
using Backend.Data.QueryHandlers.Rating;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Abstractions.Queries;
using Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("game")]
public class GameController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher
    ) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllGames([FromQuery] int limit, [FromQuery] int offset = 0)
    {
        var query = new GetAllGames { Limit = limit, Offset = offset};
        var result = await queryDispatcher.DispatchAsync<GetAllGames, List<GetAllGamesDto>>(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGame(CreateGameCommand cmd)
    {
        var userId = long.Parse(User.FindFirstValue(CustomClaimTypes.UserId)!,
            NumberStyles.Any, CultureInfo.InvariantCulture);
        cmd.UserId = userId;
        
        var result = await commandDispatcher.DispatchAsync<CreateGameCommand, long>(cmd);
        return Ok(result);
    }
    
    [HttpGet("rating")]
    public async Task<IActionResult> GetRating()
    {
        var userId = long.Parse(User.FindFirstValue(CustomClaimTypes.UserId)!,
            NumberStyles.Any, CultureInfo.InvariantCulture);
        var query = new GetRating { UserId = userId };
        var result = await queryDispatcher.DispatchAsync<GetRating, long>(query);
        return Ok(result);
    }
}