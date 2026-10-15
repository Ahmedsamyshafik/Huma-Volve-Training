using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Api.Common;
using Store.Api.Dtos.Products;
using Store.Api.Dtos.Products.V2;
using Store.Business.Models;
using Store.Business.Services;

namespace Store.Api.Controllers.V2;

/// <summary>
/// v2 of the products API. It uses the same <see cref="ProductService"/> methods as v1 -
/// only the response DTOs differ (v2 adds CategoryName), so no business logic is duplicated.
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailsDto>> Create([FromBody] CreateProductDto dto)
    {
        var created = await _service.CreateAsync(dto.Name, dto.Description, dto.Price, dto.CategoryId);
        if (created is null)
        {
            return NotFound();
        }

        var response = ProductDetailsDto.From(created);

        return CreatedAtAction(nameof(GetById), new { version = "2.0", id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        Pagination.Validate(ModelState, pageNumber, pageSize);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // v2 reads the richer projection: product fields plus the category name in one query.
        var page = await _service.GetListWithCategoryAsync(pageNumber, pageSize);
        var items = page.Items.Select(ProductListItemDto.From).ToList();

        return Ok(new PagedResult<ProductListItemDto>(items, page.TotalCount, page.PageNumber, page.PageSize));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailsDto>> GetById(int id)
    {
        var product = await _service.GetDetailsWithCategoryAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(ProductDetailsDto.From(product));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDetailsDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(
                id,
                dto.Name,
                dto.Description,
                dto.Price,
                dto.CategoryId,
                dto.RowVersion);

            if (updated is null)
            {
                return NotFound();
            }

            return Ok(ProductDetailsDto.From(updated));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The product was modified by another request. Reload it and try again." });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.SoftDeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        // Soft delete: the row stays in the table with IsDeleted = true.
        return NoContent();
    }
}
