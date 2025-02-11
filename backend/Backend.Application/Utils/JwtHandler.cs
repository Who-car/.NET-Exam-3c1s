using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Application.Common.Strings;
using Backend.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Application.Utils;

public static class JwtHandler
{
    public static string CreateJwtToken(string secret, User user)
    {
        var claims = CreateClaimsByUser(user);
        return CreateJwtToken(secret, claims);
    }
    
    public static string CreateJwtToken(string secret, IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256
            )
        );
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    public static List<Claim> CreateClaimsByUser(User user)
    {
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.UserId, user.Id.ToString()),
            new(CustomClaimTypes.Username, user.Username),
        };

        return claims;
    }
}