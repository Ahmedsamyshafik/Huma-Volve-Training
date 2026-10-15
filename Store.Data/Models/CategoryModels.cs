namespace Store.Data.Models;

/// <summary>Category list row.</summary>
public record CategoryListItemModel(int Id, string Name);

/// <summary>Category details row (includes the rowversion needed by updates).</summary>
public record CategoryDetailsModel(
    int Id,
    string Name,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    byte[] RowVersion);
