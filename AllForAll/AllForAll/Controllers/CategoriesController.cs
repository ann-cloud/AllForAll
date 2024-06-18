
using BusinessLogic.Interfaces;
using BusinessLogic.Dto.Category;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Implementation;
using AllForAll.Models;
using Microsoft.EntityFrameworkCore;


namespace AllForAll.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IPhotoService _photoService;


        public CategoriesController(IPhotoService photoService, ICategoryService categoryService)
        {
            _photoService = photoService;
            _categoryService = categoryService;
        }


       

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequestDto categoryDto, [FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(file);
                if (uploadResult.Error != null)
                {
                    return BadRequest("Failed to upload photo");
                }
                categoryDto.CategoryPhotoLink = uploadResult.SecureUrl.AbsoluteUri;
            }

            var categoryId = await _categoryService.CreateCategoryAsync(categoryDto, cancellationToken);

            if (categoryId == 0)
            {
                return BadRequest("Failed to create category");
            }

            return Ok(categoryId);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] CategoryRequestDto categoryDto, CancellationToken cancellationToken)
        {
            await _categoryService.UpdateCategoryAsync(id, categoryDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
        [HttpPost("upload-photo/{categoryId}")]
        public async Task<IActionResult> UploadCategoryPhoto(int categoryId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is empty");
            }

            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            var uploadResult = await _photoService.AddPhotoAsync(file);
            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload photo");
            }

            category.CategoryPhotoLink = uploadResult.SecureUrl.AbsoluteUri;

            await _categoryService.UpdateCategoryAsync(categoryId, new CategoryRequestDto { CategoryPhotoLink = category.CategoryPhotoLink });

            return Ok("Category photo uploaded successfully");
        }
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularCategories(CancellationToken cancellationToken)
        {
            var popularCategories = await _categoryService.GetPopularCategoriesAsync(cancellationToken);
            return Ok(popularCategories);
        }


    }
}
