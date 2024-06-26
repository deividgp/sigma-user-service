namespace Infrastructure.Services;

public class TokenService(
    IConfiguration config,
    IHttpContextAccessor httpContextAccessor,
    JwtSettings jwtSettings
) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings;
    private readonly IConfiguration _config = config;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetClaim(string claim)
    {
        JwtSecurityToken? token = GetTokenValidated();

        if (token == null)
            return null;

        return token.Claims.First(x => x.Type.Contains(claim)).Value;
    }

    public string? GetTokenFromHeader()
    {
        _httpContextAccessor!.HttpContext!.Request.Headers.TryGetValue(
            "Authorization",
            out var tokenHeaders
        );
        if (string.IsNullOrWhiteSpace(tokenHeaders))
            return null;

        string[] splittedToken = tokenHeaders.ToString().Split(' ');

        return splittedToken.Length > 0 ? splittedToken[1] : null;
    }

    public JwtSecurityToken? GetTokenValidated()
    {
        string? token = GetTokenFromHeader();

        if (token is null)
            return null;

        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.AccessTokenSecret);

        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                },
                out SecurityToken validatedToken
            );

            return (JwtSecurityToken)validatedToken;
        }
        catch
        {
            return null;
        }
    }
}
