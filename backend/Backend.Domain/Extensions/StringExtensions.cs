namespace Backend.Domain.Extensions;

public static class StringExtensions
{
    public static bool IsOnlyLetterOrDigitOrUnderscore(this string? s) =>
        s?.All(c => char.IsLetter(c) || char.IsDigit(c) || c == '_') ?? false;
}