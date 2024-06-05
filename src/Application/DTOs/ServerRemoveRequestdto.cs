namespace Application.DTOs;

public class ServerRemoveRequestDTO
{
    public required Guid ServerId { get; set; }
    public List<Guid> UserIds { get; set; } = [];
}
