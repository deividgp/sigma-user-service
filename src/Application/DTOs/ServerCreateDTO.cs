namespace Application.DTOs;

public class ServerCreateDTO
{
    public Guid UserId { get; set; }
    public Guid ServerId { get; set; }
    public required string ServerName { get; set; }
    public string? Icon { get; set; }
}
