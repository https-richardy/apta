namespace Apta.WebUI.Transport;

public sealed record Pagination<TItem>
{
    public int Total { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public IReadOnlyCollection<TItem> Items { get; init; } = [];
}
