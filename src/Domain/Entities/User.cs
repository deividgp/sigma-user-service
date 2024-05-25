namespace Domain.Entities;

public class User : Entity<Guid>
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? Avatar { get; set; }
    public List<PartialUser> Pending { get; set; } = [];
    public List<ContactUser> Contacts { get; set; } = [];
    public List<PartialUser> BlockedUsers { get; set; } = [];
    public List<PartialServer> Servers { get; set; } = [];
    public Settings Settings { get; set; } = new Settings();
}
