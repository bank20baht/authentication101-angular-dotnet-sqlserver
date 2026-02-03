using System.Diagnostics.CodeAnalysis;

namespace Shared.Dto;

[ExcludeFromCodeCoverage]
public class AddSuccessResponse
{
    public SuccessHeaderResponse status { get; set; } = null!;
    public AddSuccessResponseResult result { get; set; } = null!;
}
[ExcludeFromCodeCoverage]
public class SuccessHeaderResponse
{
    public string code { get; set; } = null!;
    public string desc { get; set; } = null!;
}

[ExcludeFromCodeCoverage]
public class AddSuccessResponseResult
{
    public string id { get; set; } = null!;
}