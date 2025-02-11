using Backend.Domain.Entities;

namespace Backend.WebAPI.Common.Models;

public class UserModel
{
    public long Id { get; set; }
    public string Username { get; set; }

    public UserModel() { }

    public UserModel(User user)
    {
        Id = user.Id;
        Username = user.Username;
    }
}