namespace CorporateSite.Domain.Entities;

public class AppUser
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Roles { get; set; } = "Editor";
    public bool IsActive { get; set; } = true;
}
