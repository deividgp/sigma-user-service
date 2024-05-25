namespace Domain.Entities;

public class Settings : Entity<Guid>
{
    public Theme Theme { get; set; } = Theme.DARK;
}
