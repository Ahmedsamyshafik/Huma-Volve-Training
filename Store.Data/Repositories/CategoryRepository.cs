using Microsoft.EntityFrameworkCore;
using Store.Data.Entities;
using Store.Data.Models;

namespace Store.Data.Repositories;

/// <summary>
/// Entity-specific repository for Category. Uses EF Core directly against the injected
/// <see cref="AppDbContext"/> (no generic repository, no unit of work).
/// </summary>
public class CategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<CategoryListItemModel> Items, int TotalCount)> GetListItemPageAsync(int pageNumber, int pageSize)
    {
        // AsNoTracking: read-only query, no change tracker needed.
        var query = _dbContext.Categories.AsNoTracking();

        var totalCount = await query.CountAsync();

        // Ordering + Skip/Take run in the database; Select projects only the two columns
        // the list contract needs, so nothing is materialised before the page is built.
        var items = await query
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryListItemModel(c.Id, c.Name))
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<CategoryDetailsModel?> GetDetailsAsync(int id)
    {
        // AsNoTracking + Select projection: only the fields the details contract needs
        // (including RowVersion for optimistic concurrency) are transferred.
        // The global query filter hides soft-deleted categories -> deleted id yields null (-> 404).
        return await _dbContext.Categories.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDetailsModel(
                c.Id,
                c.Name,
                c.CreatedAt,
                c.ModifiedAt,
                c.RowVersion))
            .FirstOrDefaultAsync();
    }

    public async Task<Category?> GetByIdTrackedAsync(int id)
    {
        // Tracked on purpose so SaveChanges can detect the service's changes (no AsNoTracking).
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        // Respects the global soft-delete filter: soft-deleted categories count as non-existing.
        return await _dbContext.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task AddAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);

        // AppDbContext.SaveChanges stamps CreatedAt (UTC) for added entities.
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category, byte[] originalRowVersion)
    {
        // Optimistic concurrency: the UPDATE targets the row version the client originally
        // read; a mismatch (concurrent update) makes EF throw DbUpdateConcurrencyException.
        _dbContext.Entry(category).Property(c => c.RowVersion).OriginalValue = originalRowVersion;

        await _dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
