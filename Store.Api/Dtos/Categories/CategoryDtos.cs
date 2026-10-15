using System.ComponentModel.DataAnnotations;

namespace Store.Api.Dtos.Categories;

public class CreateCategoryDto
{
    [Required]
    [StringLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryDto : CreateCategoryDto
{
    /// <summary>Original row version read by the client. A stale value results in HTTP 409.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = null!;
}

public class CategoryListItemDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public static CategoryListItemDto From(Store.Data.Models.CategoryListItemModel model) =>
        new() { Id = model.Id, Name = model.Name };
}

public class CategoryDetailsDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? ModifiedAt { get; init; }

    public byte[] RowVersion { get; init; } = Array.Empty<byte>();

    public static CategoryDetailsDto From(Store.Data.Models.CategoryDetailsModel model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name,
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            RowVersion = model.RowVersion,
        };
}
