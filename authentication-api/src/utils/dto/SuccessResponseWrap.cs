using System.Diagnostics.CodeAnalysis;

namespace Shared.Dto;

[ExcludeFromCodeCoverage]
public class SuccessListResponseWrap<T>
{
    public SuccessHeaderResponse status { get; set; } = null!;
    public List<T> result { get; set; } = [];
}
[ExcludeFromCodeCoverage]
public class SuccessGetResponseWrap<T>
{
    public SuccessHeaderResponse status { get; set; } = null!;
    public T? result { get; set; }
}
[ExcludeFromCodeCoverage]
public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}