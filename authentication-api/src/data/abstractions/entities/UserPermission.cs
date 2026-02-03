namespace AuthenticationService.Entity;

public class UserPermission
{
    public string username { get; set; } = null!;
    public string refresh_token { get; set; } = null!;
    public string? allow_function { get; set; }
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
}