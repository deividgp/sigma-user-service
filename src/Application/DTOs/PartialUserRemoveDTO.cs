namespace Application.DTOs;

public class PartialUserRemoveDTO
{
    public Guid UserId { get; set; }
    public Guid TargetUserId { get; set; }
}
