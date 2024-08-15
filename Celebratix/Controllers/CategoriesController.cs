using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
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
    public async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        return await _service.GetCategoriesAsDtos();
    }
}
