namespace Domain.Exceptions;

public class TokenNotFoundException : Exception
{
    public TokenNotFoundException()
        : base("JWT Token not found") { }

    public TokenNotFoundException(string message)
        : base(message) { }

    public static TokenNotFoundException Instance { get; } = new();
}
