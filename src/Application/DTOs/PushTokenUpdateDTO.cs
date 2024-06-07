namespace Application.DTOs;

public class PushTokenUpdateDTO
{
    public Guid UserId { get; set; }
    public required string PushToken { get; set; }
}
