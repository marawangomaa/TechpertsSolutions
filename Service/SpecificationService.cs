using Core.DTOs.Specifications;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
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
            return SpecificationMapper.MapToSpecificationDTOList(specs);
        }

        public async Task<SpecificationDTO?> GetSpecificationByIdAsync(string id)
        {
            var spec = await _specRepo.GetByIdWithIncludesAsync(id, s => s.Product);
            return SpecificationMapper.MapToSpecificationDTO(spec);
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

            return SpecificationMapper.MapToSpecificationDTOList(specs);
        }

        public async Task<SpecificationDTO?> CreateSpecificationAsync(CreateSpecificationDTO dto)
        {
            var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return null;

            var spec = SpecificationMapper.MapToSpecification(dto);

            await _specRepo.AddAsync(spec);
            await _specRepo.SaveChangesAsync();

            return SpecificationMapper.MapToSpecificationDTO(spec);
        }

        public async Task<bool> UpdateSpecificationAsync(UpdateSpecificationDTO dto)
        {
            var spec = await _specRepo.GetByIdAsync(dto.Id);
            if (spec == null) return false;

            var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return false;

            SpecificationMapper.MapToSpecification(dto, spec);

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