namespace API.Controllers;

[ApiController]
public class UserController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("/api/User/Create")]
    public async Task<ActionResult> CreateUser()
    {
        UserCreateDTO user =
            new()
            {
                Id = Guid.Parse(_tokenService.GetClaim("sid")!),
                Username = _tokenService.GetClaim("name")!,
                Email = _tokenService.GetClaim("emailaddress")!
            };

        return Ok(await _userService.CreateUser(user));
    }

    [HttpGet("/api/User/Get")]
    public async Task<ActionResult> GetUser()
    {
        return Ok(await _userService.GetUser(Guid.Parse(_tokenService.GetClaim("sid")!)));
    }
}
