namespace Application.DTOs;

public class PartialUserCreateDTO
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public Guid TargetUserId { get; set; }
    public string? TargetUsername { get; set; }
}
