using Backend.Domain.Abstractions.Commands;

namespace Backend.Application.Commands.SignIn;

public class SignInCommand : ICommand<string>
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}