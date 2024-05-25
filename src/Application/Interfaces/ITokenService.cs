namespace Application.Interfaces;

public interface ITokenService
{
    string? GetClaim(string claim);
    string? GetTokenFromHeader();
}
