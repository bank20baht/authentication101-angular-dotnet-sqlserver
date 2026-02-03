using System.Diagnostics.CodeAnalysis;

namespace Shared.Dto;

[ExcludeFromCodeCoverage]
public class ProblemObjectResponse
{
    public ProblemDetailResponse status { get; set; } = null!;
}
[ExcludeFromCodeCoverage]
public class ProblemBadRequestObjectResponse
{
    public List<string> errors { get; set; }
}

[ExcludeFromCodeCoverage]
public class ProblemDetailResponse
{
    public string code { get; set; } = null!;
    public string desc { get; set; } = null!;
}
