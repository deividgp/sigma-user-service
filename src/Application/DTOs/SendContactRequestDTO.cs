namespace Application.DTOs;

public class SendContactRequestDTO
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string TargetUsername { get; set; }
}
