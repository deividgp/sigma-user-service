namespace API.Helpers;

public class JwtMiddleware(RequestDelegate next, IConfiguration config)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _config = config;

    public async Task Invoke(HttpContext context, ITokenService tokenService)
    {
        var token = tokenService.GetTokenFromHeader() ?? throw new TokenNotFoundException();

        HttpClient httpClient = new();
        string url = _config.GetValue<string>("Security:Url") + "ValidateToken";
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        var response = await httpClient.PostAsJsonAsync(url, "");
        response.EnsureSuccessStatusCode();
        bool result = bool.Parse(await response.Content.ReadAsStringAsync());

        if (!result)
        {
            throw new TokenInvalidException();
        }

        await _next(context);
    }
}
