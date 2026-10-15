using Store.Business.Models;
using Store.Data.Entities;
using Store.Data.Models;
using Store.Data.Repositories;

namespace Store.Business.Services;

/// <summary>
/// Business operations for Product. Both API versions call the same methods, so the
/// versions differ only in which DTO they map the results to - no duplicated business logic.
/// </summary>
public class ProductService
{
    private readonly ProductRepository _products;
    private readonly CategoryRepository _categories;

    public ProductService(ProductRepository products, CategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<PagedResult<ProductListItemModel>> GetListAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _products.GetListItemPageAsync(pageNumber, pageSize);
        return new PagedResult<ProductListItemModel>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<ProductListItemWithCategoryModel>> GetListWithCategoryAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _products.GetListItemWithCategoryPageAsync(pageNumber, pageSize);
        return new PagedResult<ProductListItemWithCategoryModel>(items, totalCount, pageNumber, pageSize);
    }

    public Task<ProductDetailsModel?> GetDetailsAsync(int id) => _products.GetDetailsAsync(id);

    public Task<ProductDetailsWithCategoryModel?> GetDetailsWithCategoryAsync(int id) => _products.GetDetailsWithCategoryAsync(id);

    public async Task<ProductDetailsWithCategoryModel?> CreateAsync(string name, string? description, decimal price, int categoryId)
    {
        // A product may only reference an existing (and not soft-deleted) category.
        if (!await _categories.ExistsAsync(categoryId))
        {
            return null;
        }

        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            CategoryId = categoryId,
        };

        await _products.AddAsync(product);

        // Re-read as a projection so the response carries the database-generated row version.
        return await _products.GetDetailsWithCategoryAsync(product.Id);
    }

    public async Task<ProductDetailsWithCategoryModel?> UpdateAsync(
        int id,
        string name,
        string? description,
        decimal price,
        int categoryId,
        byte[] originalRowVersion)
    {
        var product = await _products.GetByIdTrackedAsync(id);
        if (product is null)
        {
            return null;
        }

        if (!await _categories.ExistsAsync(categoryId))
        {
            return null;
        }

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.CategoryId = categoryId;

        // Throws DbUpdateConcurrencyException when another request saved a newer version
        // in the meantime; the API turns that into HTTP 409 instead of silently overwriting.
        await _products.UpdateAsync(product, originalRowVersion);

        return await _products.GetDetailsWithCategoryAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var product = await _products.GetByIdTrackedAsync(id);
        if (product is null)
        {
            return false;
        }

        product.IsDeleted = true;
        await _products.SaveChangesAsync();
        return true;
    }
}
