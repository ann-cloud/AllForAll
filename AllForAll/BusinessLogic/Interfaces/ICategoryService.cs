
using AllForAll.Models;
using BusinessLogic.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICategoryService
    {
        Task<ICollection<Category>> GetAllCategoriesAsync(CancellationToken cancellation = default);
        Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellation = default);

        Task<bool> IsCategoryExistAsync(int id, CancellationToken cancellation = default);

        Task<int> CreateCategoryAsync(CategoryRequestDto category, CancellationToken cancellation = default);

        Task UpdateCategoryAsync(int id, CategoryRequestDto category, CancellationToken cancellation = default);

        Task DeleteCategoryAsync(int id, CancellationToken cancellation = default);

        Task<List<Category>> GetPopularCategoriesAsync(CancellationToken cancellationToken);
    }
}
