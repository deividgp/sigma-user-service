namespace Domain.Exceptions;

public class TokenInvalidException : Exception
{
    public const int REFRESH_TOKEN_CODE = 477;

    public TokenInvalidException()
        : base("Invalid JWT Token") { }

    public TokenInvalidException(string message)
        : base(message) { }

    public static TokenInvalidException Instance { get; } = new();
}
