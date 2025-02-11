using System.Security.Cryptography;
using System.Text;

namespace Backend.Application.Utils;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;
    
    private static Rfc2898DeriveBytes CreatePbkdf2(string pass, byte[] salt) => 
        new (Encoding.UTF8.GetBytes(pass), salt, Iterations, HashAlgorithmName.SHA256);

    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var pbkdf2 = CreatePbkdf2(password, salt);
        var hash = pbkdf2.GetBytes(HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        var hashedPassword = Convert.ToBase64String(hashBytes);
        return hashedPassword;
    }

    public static bool VerifyPassword(string? password, string hashedPassword)
    {
        if (password is null) return false;
        
        var hashBytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var pbkdf2 = CreatePbkdf2(password, salt);
        var hash = pbkdf2.GetBytes(HashSize);

        for (var i = 0; i < HashSize; i++)
            if (hashBytes[i + SaltSize] != hash[i])
                return false;

        return true;
    }
}