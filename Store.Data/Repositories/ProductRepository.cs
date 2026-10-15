using Microsoft.EntityFrameworkCore;
using Store.Data.Entities;
using Store.Data.Models;

namespace Store.Data.Repositories;

/// <summary>
/// Entity-specific repository for Product. Uses EF Core directly against the injected
/// <see cref="AppDbContext"/> (no generic repository, no unit of work).
/// </summary>
public class ProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<ProductListItemModel> Items, int TotalCount)> GetListItemPageAsync(int pageNumber, int pageSize)
    {
        // AsNoTracking: this is a read-only query, so the change tracker is not needed.
        var query = _dbContext.Products.AsNoTracking();

        var totalCount = await query.CountAsync();

        // Deterministic ordering, then paging in SQL (Skip/Take) BEFORE anything is materialised.
        // The Select projects straight into the small response model, so only the three columns
        // the v1 list needs are transferred - no Include() and no full Product entity.
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemModel(p.Id, p.Name, p.Price))
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<ProductListItemWithCategoryModel> Items, int TotalCount)> GetListItemWithCategoryPageAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Products.AsNoTracking();

        var totalCount = await query.CountAsync();

        // N+1 problem prevented: the category name is fetched by the SAME SQL statement
        // (the Select produces a JOIN), so a page of 10 products costs one round trip -
        // not one query for the products plus one query per product for its category.
        // There is no lazy loading and no Include(): Include() would load whole Product and
        // Category entities, while this projection transfers only the four required columns.
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemWithCategoryModel(p.Id, p.Name, p.Price, p.Category.Name))
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ProductDetailsModel?> GetDetailsAsync(int id)
    {
        // AsNoTracking + Select projection: only the columns the v1 details contract needs
        // are selected, so there is no need for Include(p => p.Category) and no entity is tracked.
        // The global query filter hides soft-deleted products, so a deleted id yields null (-> 404).
        return await _dbContext.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsModel(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.CreatedAt,
                p.ModifiedAt,
                p.RowVersion))
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDetailsWithCategoryModel?> GetDetailsWithCategoryAsync(int id)
    {
        // Single-query projection: the category name comes from a JOIN inside this same
        // statement (N+1 avoided) and only the columns used by the response are transferred.
        return await _dbContext.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsWithCategoryModel(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Category.Name,
                p.CreatedAt,
                p.ModifiedAt,
                p.RowVersion))
            .FirstOrDefaultAsync();
    }

    public async Task<Product?> GetByIdTrackedAsync(int id)
    {
        // Tracked on purpose: the business service mutates this instance and SaveChanges
        // must detect the changes to build the UPDATE. AsNoTracking would break that,
        // so this is the one query that intentionally keeps tracking.
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);

        // AppDbContext.SaveChanges stamps CreatedAt (UTC) for added entities.
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product, byte[] originalRowVersion)
    {
        // Optimistic concurrency: force the UPDATE's WHERE clause to use the row version the
        // client originally read instead of the one just loaded from the database. If another
        // request saved in between, the database row version no longer matches, no row is
        // updated and EF Core throws DbUpdateConcurrencyException (the API maps that to 409).
        // The product must be the tracked instance returned by GetByIdTrackedAsync.
        _dbContext.Entry(product).Property(p => p.RowVersion).OriginalValue = originalRowVersion;

        await _dbContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
