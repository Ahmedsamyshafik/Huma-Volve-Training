using Store.Data.Entities;

namespace Store.Data.Seed;

/// <summary>
/// Deterministic model seed data (EF Core <c>HasData</c>): 6 categories and 25 products,
/// so the paginated list endpoints have 3 pages at the default page size of 10.
/// Ids and timestamps are fixed so migrations stay deterministic.
/// </summary>
public static class SeedData
{
    private static readonly DateTime CreatedAt = new(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc);

    public static readonly Category[] Categories =
    [
        new Category { Id = 1, Name = "Electronics", CreatedAt = CreatedAt },
        new Category { Id = 2, Name = "Books", CreatedAt = CreatedAt },
        new Category { Id = 3, Name = "Clothing", CreatedAt = CreatedAt },
        new Category { Id = 4, Name = "Sports", CreatedAt = CreatedAt },
        new Category { Id = 5, Name = "Toys", CreatedAt = CreatedAt },
        new Category { Id = 6, Name = "Home & Kitchen", CreatedAt = CreatedAt },
    ];

    public static readonly Product[] Products =
    [
        new Product { Id = 1, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse.", Price = 29.99m, CategoryId = 1, CreatedAt = CreatedAt },
        new Product { Id = 2, Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard.", Price = 89.50m, CategoryId = 1, CreatedAt = CreatedAt },
        new Product { Id = 3, Name = "27-inch Monitor", Description = "QHD IPS monitor.", Price = 249.00m, CategoryId = 1, CreatedAt = CreatedAt },
        new Product { Id = 4, Name = "USB-C Hub", Description = "7-in-1 USB-C hub.", Price = 45.00m, CategoryId = 1, CreatedAt = CreatedAt },
        new Product { Id = 5, Name = "Bluetooth Speaker", Description = "Portable speaker, 12h battery.", Price = 59.99m, CategoryId = 1, CreatedAt = CreatedAt },
        new Product { Id = 6, Name = "The Great Gatsby", Description = "Classic novel.", Price = 12.99m, CategoryId = 2, CreatedAt = CreatedAt },
        new Product { Id = 7, Name = "Clean Code", Description = "Handbook of agile software craftsmanship.", Price = 39.99m, CategoryId = 2, CreatedAt = CreatedAt },
        new Product { Id = 8, Name = "Dune", Description = "Science fiction epic.", Price = 15.50m, CategoryId = 2, CreatedAt = CreatedAt },
        new Product { Id = 9, Name = "Designing Data-Intensive Applications", Description = "The big ideas behind reliable systems.", Price = 49.99m, CategoryId = 2, CreatedAt = CreatedAt },
        new Product { Id = 10, Name = "The Pragmatic Programmer", Description = "Your journey to mastery.", Price = 42.00m, CategoryId = 2, CreatedAt = CreatedAt },
        new Product { Id = 11, Name = "Cotton T-Shirt", Description = "100% cotton crew neck.", Price = 18.99m, CategoryId = 3, CreatedAt = CreatedAt },
        new Product { Id = 12, Name = "Denim Jeans", Description = "Straight-fit jeans.", Price = 59.00m, CategoryId = 3, CreatedAt = CreatedAt },
        new Product { Id = 13, Name = "Winter Jacket", Description = "Waterproof insulated jacket.", Price = 99.00m, CategoryId = 3, CreatedAt = CreatedAt },
        new Product { Id = 14, Name = "Running Shoes", Description = "Lightweight running shoes.", Price = 74.50m, CategoryId = 3, CreatedAt = CreatedAt },
        new Product { Id = 15, Name = "Baseball Cap", Description = null, Price = 14.00m, CategoryId = 3, CreatedAt = CreatedAt },
        new Product { Id = 16, Name = "Soccer Ball", Description = "Official size 5 ball.", Price = 24.99m, CategoryId = 4, CreatedAt = CreatedAt },
        new Product { Id = 17, Name = "Yoga Mat", Description = "Non-slip 6mm mat.", Price = 27.50m, CategoryId = 4, CreatedAt = CreatedAt },
        new Product { Id = 18, Name = "Dumbbell Set", Description = "Adjustable 20kg set.", Price = 120.00m, CategoryId = 4, CreatedAt = CreatedAt },
        new Product { Id = 19, Name = "Tennis Racket", Description = "Graphite frame racket.", Price = 89.00m, CategoryId = 4, CreatedAt = CreatedAt },
        new Product { Id = 20, Name = "Jump Rope", Description = "Speed rope with bearings.", Price = 9.99m, CategoryId = 4, CreatedAt = CreatedAt },
        new Product { Id = 21, Name = "Building Blocks Set", Description = "120-piece brick set.", Price = 34.99m, CategoryId = 5, CreatedAt = CreatedAt },
        new Product { Id = 22, Name = "Remote Control Car", Description = "4WD off-road RC car.", Price = 49.99m, CategoryId = 5, CreatedAt = CreatedAt },
        new Product { Id = 23, Name = "Plush Teddy Bear", Description = "40cm soft teddy bear.", Price = 16.50m, CategoryId = 5, CreatedAt = CreatedAt },
        new Product { Id = 24, Name = "Coffee Maker", Description = "12-cup drip coffee maker.", Price = 79.99m, CategoryId = 6, CreatedAt = CreatedAt },
        new Product { Id = 25, Name = "Non-stick Frying Pan", Description = "28cm induction pan.", Price = 28.00m, CategoryId = 6, CreatedAt = CreatedAt },
    ];
}
