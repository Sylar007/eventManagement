using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Admin.Categories;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers.Admin;

/// <summary>
/// Admin specific endpoints for managing categories
/// See "public" CategoriesController for non-admin endpoints
/// </summary>
[Area("admin")]
[Route("[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.SuperAdmin)]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _service;

    public CategoriesController(CategoryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gives back a list of all countries (both active and inactive)
    /// </summary>
    [HttpGet]
    public async Task<IEnumerable<CategoryAdminDto>> GetCategories()
    {
        return await _service.GetCategoriesAsAdminDtos();
    }

    [HttpPost]
    public async Task<ActionResult<CategoryAdminDto>> CreateCategory(string id, CategoryUpdateAdminDto dto)
    {
        return await _service.CreateCategory(dto);
    }

    /// <response code="404">No category with given id was found</response>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryAdminDto>> UpdateCategory(int id, CategoryUpdateAdminDto dto)
    {
        return await _service.UpdateCategory(id, dto);
    }
}
