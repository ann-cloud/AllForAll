using AllForAll.Models;
using AutoMapper;
using BusinessLogic.Dto.Manufacturer;
using BusinessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.Implementation
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly AllForAllDbContext _dbContext;
        private readonly IMapper _mapper;

        public ManufacturerService(AllForAllDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> CreateManufacturerAsync(ManufacturerRequestDto manufacturer, CancellationToken cancellation = default)
        {
            var mappedManufacturer = _mapper.Map<Manufacturer>(manufacturer);
            var createdManufacturer = await _dbContext.Manufacturers.AddAsync(mappedManufacturer, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return createdManufacturer.Entity.ManufacturerId;
        }

        public async Task DeleteManufacturerAsync(int id, CancellationToken cancellation = default)
        {
            var manufacturerToDelete = await _dbContext.Manufacturers.FindAsync(id, cancellation);
            if (manufacturerToDelete != null)
            {
                _dbContext.Manufacturers.Remove(manufacturerToDelete);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }

        public async Task<ICollection<Manufacturer>> GetAllManufacturersAsync(CancellationToken cancellation = default)
        {
            return await _dbContext.Manufacturers.ToListAsync(cancellation);
        }

        public async Task<Manufacturer> GetManufacturerByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.Manufacturers.FirstOrDefaultAsync(m => m.ManufacturerId == id, cancellation);
        }

        public async Task<bool> IsManufacturerExistAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbContext.Manufacturers.AnyAsync(m => m.ManufacturerId == id, cancellation);
        }

        public async Task UpdateManufacturerAsync(int id, ManufacturerRequestDto manufacturer, CancellationToken cancellation = default)
        {
            var manufacturerToUpdate = await _dbContext.Manufacturers.FirstOrDefaultAsync(m => m.ManufacturerId == id, cancellation);
            if (manufacturerToUpdate != null)
            {
                _mapper.Map(manufacturer, manufacturerToUpdate);
                _dbContext.Update(manufacturerToUpdate);
                await _dbContext.SaveChangesAsync(cancellation);
            }
        }
        public async Task<List<Manufacturer>> GetPopularManufacturersAsync(CancellationToken cancellationToken)
        {
            var popularManufacturers = await _dbContext.Manufacturers
                .Select(m => new
                {
                    Manufacturer = m,
                    FeedbackCount = m.Products.SelectMany(p => p.Feedbacks).Count()
                })
                .OrderByDescending(x => x.FeedbackCount)
                .Take(5)
                .Select(x => x.Manufacturer)
                .ToListAsync(cancellationToken);

            return popularManufacturers;
        }
    }
}
