using Backend.Application.Commands.Options;
using Backend.Application.Common.Strings;
using Backend.Application.Utils;
using Backend.Data;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Entities;
using Backend.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Application.Commands.SignUp;

public class SignUpCommandHandler(
    IOptions<AuthenticationOptions> opts,
    PostgresDbContext postgres
    ) : ICommandHandler<SignUpCommand, string>
{
    public async Task<string> HandleAsync(SignUpCommand command)
    {
        var dbUser = await postgres.Users.FirstOrDefaultAsync(u => u.Username == command.Username);
        if (dbUser is not null) 
            throw new ArgumentException(Errors.Registration.UserAlreadyExists);

        if (command.Username is null || !command.Username.IsOnlyLetterOrDigitOrUnderscore())
            throw new ArgumentException(Errors.Registration.InvalidUsername);
        
        if (command.Password is null || command.Password.Length < 8)
            throw new ArgumentException(Errors.Registration.InvalidPassword);

        var user = new User
        {
            Username = command.Username,
            Password = PasswordHasher.HashPassword(command.Password)
        };

        await postgres.Users.AddAsync(user);
        await postgres.SaveChangesAsync();
        
        return JwtHandler.CreateJwtToken(opts.Value.Secret, user);
    }
}