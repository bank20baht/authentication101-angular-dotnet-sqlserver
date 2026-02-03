namespace AuthenticationService.Dto;

public class AuthenticationRequestBodyDto
{
    public string username { get; set; } = null!;
    public string password { get; set; } = null!;
}