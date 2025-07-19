using Core.DTOs.Specifications;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class SpecificationService : ISpecificationService
    {
        private readonly IRepository<Specification> _specRepo;
        private readonly IRepository<Product> _productRepo;

        public SpecificationService(IRepository<Specification> specRepo, IRepository<Product> productRepo)
        {
            _specRepo = specRepo;
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<SpecificationDTO>> GetAllSpecificationsAsync()
        {
            var specs = await _specRepo.GetAllWithIncludesAsync(s => s.Product);
            return specs.Select(s => new SpecificationDTO
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                ProductId = s.ProductId,
                ProductName = s.Product?.Name
            });
        }

        public async Task<SpecificationDTO?> GetSpecificationByIdAsync(string id)
        {
            var spec = await _specRepo.GetByIdWithIncludesAsync(id, s => s.Product);
            if (spec == null) return null;

            return new SpecificationDTO
            {
                Id = spec.Id,
                Key = spec.Key,
                Value = spec.Value,
                ProductId = spec.ProductId,
                ProductName = spec.Product?.Name
            };
        }

        public async Task<IEnumerable<SpecificationDTO>> GetSpecificationsByProductIdAsync(string productId)
        {
            var productExists = await _productRepo.AnyAsync(p => p.Id == productId);
            if (!productExists)
                return Enumerable.Empty<SpecificationDTO>();

            var specs = await _specRepo.FindWithIncludesAsync(
                s => s.ProductId == productId,
                s => s.Product
            );

            return specs.Select(s => new SpecificationDTO
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                ProductId = s.ProductId,
                ProductName = s.Product?.Name
            });
        }

        public async Task<SpecificationDTO?> CreateSpecificationAsync(CreateSpecificationDTO dto)
        {
            var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return null;

            var spec = new Specification
            {
                Id = Guid.NewGuid().ToString(),
                Key = dto.Key,
                Value = dto.Value,
                ProductId = dto.ProductId
            };

            await _specRepo.AddAsync(spec);
            await _specRepo.SaveChangesAsync();

            return new SpecificationDTO
            {
                Id = spec.Id,
                Key = spec.Key,
                Value = spec.Value,
                ProductId = spec.ProductId
            };
        }

        public async Task<bool> UpdateSpecificationAsync(UpdateSpecificationDTO dto)
        {
            var spec = await _specRepo.GetByIdAsync(dto.Id);
            if (spec == null) return false;

            var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return false;

            spec.Key = dto.Key;
            spec.Value = dto.Value;
            spec.ProductId = dto.ProductId;

            _specRepo.Update(spec);
            await _specRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSpecificationAsync(string id)
        {
            var spec = await _specRepo.GetByIdAsync(id);
            if (spec == null) return false;

            _specRepo.Remove(spec);
            await _specRepo.SaveChangesAsync();

            return true;
        }
    }
}