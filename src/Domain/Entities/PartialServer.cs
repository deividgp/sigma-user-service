namespace Domain.Entities;

public class PartialServer : Entity<Guid>
{
    public required string Name { get; set; }
    public string? Icon { get; set; }
}
