namespace Store.Data.Models;

// Lightweight read models used as Select() projection targets by the repositories.
// They live in the data layer because Store.Data cannot reference Store.Api (dependency
// direction is Api -> Business -> Data); the API controllers map them to API DTOs.

/// <summary>v1 list row: only the columns the v1 list contract needs.</summary>
public record ProductListItemModel(int ProductId, string Name, decimal Price);

/// <summary>v2 list row: adds the category name, still fetched with a single JOIN.</summary>
public record ProductListItemWithCategoryModel(int ProductId, string Name, decimal Price, string CategoryName);

/// <summary>v1 details row: no category name - that is what the v2 contract adds.</summary>
public record ProductDetailsModel(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int CategoryId,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    byte[] RowVersion);

/// <summary>v2 details row (also used after create/update so the new rowversion can be returned).</summary>
public record ProductDetailsWithCategoryModel(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    byte[] RowVersion);
