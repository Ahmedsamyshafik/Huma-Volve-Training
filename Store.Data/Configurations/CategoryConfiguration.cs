using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Data.Entities;

namespace Store.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Optimistic concurrency: SQL Server rowversion column used as the concurrency token.
        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasData(Seed.SeedData.Categories);
    }
}
