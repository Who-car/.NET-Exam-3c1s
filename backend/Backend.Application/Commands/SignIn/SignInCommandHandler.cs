using Backend.Application.Commands.Options;
using Backend.Application.Common.Strings;
using Backend.Application.Utils;
using Backend.Data;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Application.Commands.SignIn;

public class SignInCommandHandler(
    IOptions<AuthenticationOptions> opts,
    PostgresDbContext postgres
    ) : ICommandHandler<SignInCommand, string>
{
    public async Task<string> HandleAsync(SignInCommand command)
    {
        var user = await postgres.Users.FirstOrDefaultAsync(u => u.Username == command.Username);
        if (user is null)
            throw new ArgumentException(Errors.Authentication.UserNotFound);
    
        if (!PasswordHasher.VerifyPassword(command.Password, user.Password))
            throw new PermissionDeniedException(Errors.Authentication.InvalidPassword);
        
        return JwtHandler.CreateJwtToken(opts.Value.Secret, user);
    }
}