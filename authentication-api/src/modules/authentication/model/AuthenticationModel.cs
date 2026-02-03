namespace AuthenticationService.Models;

public class AuthenticationModel
{
    public string OaUserName { get; set; } = null!;
    public string StaffName { get; set; } = null!;
    public string StaffStatus { get; set; } = null!;
    public string TellerId { get; set; } = null!;
    public string TerminalId { get; set; } = null!;
    public string BranchNo { get; set; } = null!;
    public IEnumerable<string> Permissions { get; set; } = Array.Empty<string>();
}
