using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Api.Common;
using Store.Api.Dtos.Categories;
using Store.Business.Models;
using Store.Business.Services;

namespace Store.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _service;

    public CategoriesController(CategoryService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDetailsDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var created = await _service.CreateAsync(dto.Name);
        var response = CategoryDetailsDto.From(created!);

        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<CategoryListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        Pagination.Validate(ModelState, pageNumber, pageSize);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var page = await _service.GetListAsync(pageNumber, pageSize);
        var items = page.Items.Select(CategoryListItemDto.From).ToList();

        return Ok(new PagedResult<CategoryListItemDto>(items, page.TotalCount, page.PageNumber, page.PageSize));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDetailsDto>> GetById(int id)
    {
        var category = await _service.GetDetailsAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(CategoryDetailsDto.From(category));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryDetailsDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto.Name, dto.RowVersion);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(CategoryDetailsDto.From(updated));
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request saved a newer version of this category between our read and write.
            return Conflict(new { message = "The category was modified by another request. Reload it and try again." });
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
