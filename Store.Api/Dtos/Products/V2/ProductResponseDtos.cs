using Store.Data.Models;

namespace Store.Api.Dtos.Products.V2;

/// <summary>v2 list contract: adds CategoryName to the v1 row.</summary>
public sealed class ProductListItemDto
{
    public int ProductId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public static ProductListItemDto From(ProductListItemWithCategoryModel model) =>
        new()
        {
            ProductId = model.ProductId,
            Name = model.Name,
            Price = model.Price,
            CategoryName = model.CategoryName,
        };
}

/// <summary>v2 details contract: the v1 details plus CategoryName.</summary>
public sealed class ProductDetailsDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? ModifiedAt { get; init; }

    public byte[] RowVersion { get; init; } = Array.Empty<byte>();

    public static ProductDetailsDto From(ProductDetailsWithCategoryModel model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            RowVersion = model.RowVersion,
        };
}
