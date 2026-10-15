using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Store.Api.Common;

/// <summary>Shared pagination request validation for the list endpoints.</summary>
public static class Pagination
{
    public const int MinPageNumber = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;

    /// <summary>Adds model errors for out-of-range page inputs; the caller returns HTTP 400 when invalid.</summary>
    public static void Validate(ModelStateDictionary modelState, int pageNumber, int pageSize)
    {
        if (pageNumber < MinPageNumber)
        {
            modelState.AddModelError(nameof(pageNumber), $"pageNumber must be at least {MinPageNumber}.");
        }

        if (pageSize < MinPageSize || pageSize > MaxPageSize)
        {
            modelState.AddModelError(nameof(pageSize), $"pageSize must be between {MinPageSize} and {MaxPageSize}.");
        }
    }
}
