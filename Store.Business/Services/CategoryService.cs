using Store.Business.Models;
using Store.Data.Entities;
using Store.Data.Models;
using Store.Data.Repositories;

namespace Store.Business.Services;

public class CategoryService
{
    private readonly CategoryRepository _categories;

    public CategoryService(CategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<PagedResult<CategoryListItemModel>> GetListAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _categories.GetListItemPageAsync(pageNumber, pageSize);
        return new PagedResult<CategoryListItemModel>(items, totalCount, pageNumber, pageSize);
    }

    public Task<CategoryDetailsModel?> GetDetailsAsync(int id) => _categories.GetDetailsAsync(id);

    public async Task<CategoryDetailsModel?> CreateAsync(string name)
    {
        var category = new Category { Name = name };
        await _categories.AddAsync(category);

        // Re-read as a projection so the response carries the database-generated row version.
        return await _categories.GetDetailsAsync(category.Id);
    }

    public async Task<CategoryDetailsModel?> UpdateAsync(int id, string name, byte[] originalRowVersion)
    {
        var category = await _categories.GetByIdTrackedAsync(id);
        if (category is null)
        {
            return null;
        }

        category.Name = name;

        // Throws DbUpdateConcurrencyException on a stale row version; the API maps it to HTTP 409.
        await _categories.UpdateAsync(category, originalRowVersion);

        return await _categories.GetDetailsAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var category = await _categories.GetByIdTrackedAsync(id);
        if (category is null)
        {
            return false;
        }

        // Soft delete: IsDeleted = true, ModifiedAt stamped by the SaveChanges override.
        category.IsDeleted = true;
        await _categories.SaveChangesAsync();
        return true;
    }
}
