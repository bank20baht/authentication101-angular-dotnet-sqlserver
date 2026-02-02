namespace Shared.Models;

public class AuthenticationModel
{
    public string UserName { get; set; } = null!;
    public IEnumerable<string> Permissions { get; set; } = Array.Empty<string>();
}
