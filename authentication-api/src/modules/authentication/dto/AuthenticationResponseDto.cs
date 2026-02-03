using System.Diagnostics.CodeAnalysis;

namespace AuthenticationService.Dto;

[ExcludeFromCodeCoverage]
public class AuthorizationResponseDto
{
    public string username { get; set; } = null!;
    public List<string> application_function { get; set; } = null!;

}
