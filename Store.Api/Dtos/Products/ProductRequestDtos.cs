using System.ComponentModel.DataAnnotations;

namespace Store.Api.Dtos.Products;

public class CreateProductDto
{
    [Required]
    [StringLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must be 1000 characters or fewer.")]
    public string? Description { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Price must be between 0.01 and 1000000.")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive value.")]
    public int CategoryId { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    /// <summary>Original row version read by the client. A stale value results in HTTP 409.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = null!;
}
