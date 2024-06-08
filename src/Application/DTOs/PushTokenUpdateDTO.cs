namespace Application.DTOs;

public class PushTokenUpdateDTO
{
    public Guid UserId { get; set; }
    public string? PushToken { get; set; }
}
