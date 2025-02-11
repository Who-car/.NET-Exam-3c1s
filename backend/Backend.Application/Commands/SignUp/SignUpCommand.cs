using Backend.Domain.Abstractions.Commands;

namespace Backend.Application.Commands.SignUp;

public class SignUpCommand : ICommand<string>
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}