using AllForAll.Models;
using BusinessLogic.Dto.Manufacturer;


namespace BusinessLogic.Interfaces
{
    public interface IManufacturerService
    {
        Task<ICollection<Manufacturer>> GetAllManufacturersAsync(CancellationToken cancellation = default);
        Task<Manufacturer> GetManufacturerByIdAsync(int id, CancellationToken cancellation = default);

        Task<bool> IsManufacturerExistAsync(int id, CancellationToken cancellation = default);

        Task<int> CreateManufacturerAsync(ManufacturerRequestDto manufacturer, CancellationToken cancellation = default);

        Task UpdateManufacturerAsync(int id, ManufacturerRequestDto manufacturer, CancellationToken cancellation = default);

        Task DeleteManufacturerAsync(int id, CancellationToken cancellation = default);

        Task<List<Manufacturer>> GetPopularManufacturersAsync(CancellationToken cancellationToken);
    }
}
