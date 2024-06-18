using AllForAll.Models;
using BusinessLogic.Dto.Product;

namespace BusinessLogic.Interfaces
{
    public interface IProductService
    {
        Task<ICollection<Product>> GetAllProductsAsync(CancellationToken cancellation = default);
        Task<Product> GetProductByIdAsync(int id, CancellationToken cancellation = default);

        Task<bool> IsProductExistAsync(int id, CancellationToken cancellation = default);

        Task<int> CreateProductAsync(ProductRequestDto product, CancellationToken cancellation = default);

        Task UpdateProductAsync(int id,ProductRequestDto product, CancellationToken cancellation = default);

        Task DeleteProductAsync(int id, CancellationToken cancellation = default);

        Task<List<Product>> GetPopularProductsAsync(CancellationToken cancellationToken);


    }
}
