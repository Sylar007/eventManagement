using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Admin.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class CategoryService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(CelebratixDbContext dbContext, ILogger<CategoryService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsDtos()
    {
        return await _dbContext.Categories.Select(c => new CategoryDto((c))).ToListAsync();
    }

    public async Task<IEnumerable<CategoryAdminDto>> GetCategoriesAsAdminDtos()
    {
        return await _dbContext.Categories.Select(c => new CategoryAdminDto((c))).ToListAsync();
    }

    public async Task<CategoryAdminDto> CreateCategory(CategoryUpdateAdminDto dto)
    {
        var newCategory = new Category
        {
            Name = dto.Name
        };

        _dbContext.Add(newCategory);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Create new category, id: {CategoryId}", newCategory.Id);
        return new CategoryAdminDto(newCategory);
    }

    public async Task<CategoryAdminDto> UpdateCategory(int id, CategoryUpdateAdminDto dto)
    {
        var category = await _dbContext.Categories.FirstOrThrowAsync(c => c.Id == id);

        _dbContext.Entry(category).CurrentValues.SetValues(dto);

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Updated category, id: {CategoryId}", category.Id);
        return new CategoryAdminDto(category);
    }
}
