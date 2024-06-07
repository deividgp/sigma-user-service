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
        User? user = await _userService.GetUser(Guid.Parse(_tokenService.GetClaim("sid")!));

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPatch("/api/User/RemoveFromServer")]
    public async Task<ActionResult> RemoveFromServer(ServerRemoveRequestDTO serverRemoveRequest)
    {
        try
        {
            foreach (Guid userId in serverRemoveRequest.UserIds)
            {
                await _userService.RemoveServer(
                    new ServerRemoveDTO()
                    {
                        UserId = userId,
                        ServerId = serverRemoveRequest.ServerId
                    }
                );
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPatch("/api/User/UpdatePushToken")]
    public async Task<ActionResult> UpdatePushToken(PushTokenUpdateDTO pushTokenUpdate)
    {
        bool result = await _userService.UpdatePushToken(new PushTokenUpdateDTO() {
            PushToken = pushTokenUpdate.PushToken,
            UserId = Guid.Parse(_tokenService.GetClaim("sid")!)
        });

        if (!result)
            return NotFound();

        return Ok();
    }
}
