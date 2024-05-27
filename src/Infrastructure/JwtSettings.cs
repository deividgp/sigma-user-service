namespace Infrastructure;

public class JwtSettings
{
    public string AccessTokenSecret { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
}
