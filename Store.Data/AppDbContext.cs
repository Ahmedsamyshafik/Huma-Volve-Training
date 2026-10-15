using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Store.Data.Configurations;
using Store.Data.Entities;

namespace Store.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());


        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var body = Expression.Equal(
                Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                Expression.Constant(false));
            entityType.SetQueryFilter(Expression.Lambda(body, parameter));
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditTimestamps();
        return base.SaveChanges();
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // New rows always get a UTC creation timestamp.
                entry.Entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Modified rows (including soft deletes) get a UTC modification timestamp...
                entry.Entity.ModifiedAt = now;

                // ...while CreatedAt must survive updates untouched, so it is never part of the UPDATE.
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }
}
