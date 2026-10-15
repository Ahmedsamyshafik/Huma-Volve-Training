namespace Store.Business.Models;

/// <summary>
/// Pagination metadata plus the page of items, returned by every list endpoint.
/// Generic over the item type so it can carry data-layer read models inside the business
/// layer as well as API DTOs in the controllers.
/// </summary>
public sealed class PagedResult<T>
{
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IReadOnlyList<T> Items { get; }

    public int TotalCount { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalPages { get; }
}
