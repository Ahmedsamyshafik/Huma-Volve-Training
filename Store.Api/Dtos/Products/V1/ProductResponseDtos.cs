using Store.Data.Models;

namespace Store.Api.Dtos.Products.V1;

/// <summary>v1 list contract: product id, name and price only.</summary>
public sealed class ProductListItemDto
{
    public int ProductId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public static ProductListItemDto From(ProductListItemModel model) =>
        new()
        {
            ProductId = model.ProductId,
            Name = model.Name,
            Price = model.Price,
        };
}

/// <summary>v1 details contract: deliberately without CategoryName - adding it is exactly what v2 does.</summary>
public sealed class ProductDetailsDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int CategoryId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? ModifiedAt { get; init; }

    public byte[] RowVersion { get; init; } = Array.Empty<byte>();

    /// <summary>Used by GET (v1 projects only the columns this contract needs).</summary>
    public static ProductDetailsDto From(ProductDetailsModel model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            RowVersion = model.RowVersion,
        };

    /// <summary>
    /// Used by POST/PUT, which share the richer service result with v2; v1's contract simply
    /// does not expose CategoryName.
    /// </summary>
    public static ProductDetailsDto From(ProductDetailsWithCategoryModel model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            RowVersion = model.RowVersion,
        };
}
