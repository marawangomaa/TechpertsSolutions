using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.Entities;
using Core.DTOs.SpecificationsDTOs;

namespace Service.Utilities
{
    public static class SpecificationMapper
    {
        public static Core.DTOs.SpecificationsDTOs.SpecificationDTO MapToSpecificationDTO(Specification specification)
        {
            if (specification == null) return null;

            var dto = new Core.DTOs.SpecificationsDTOs.SpecificationDTO
            {
                Id = specification.Id,
                Key = specification.Key,
                Value = specification.Value,
                ProductId = specification.ProductId,
                ProductName = specification.Product?.Name
            };
            return dto;
        }

        public static Specification MapToSpecification(CreateSpecificationDTO dto)
        {
            if (dto == null) return null;

            return new Specification
            {
                Id = Guid.NewGuid().ToString(),
                Key = dto.Key,
                Value = dto.Value,
                ProductId = dto.ProductId
            };
        }

        public static Specification MapToSpecification(UpdateSpecificationDTO dto, Specification existingSpecification)
        {
            if (dto == null || existingSpecification == null) return null;

            existingSpecification.Key = dto.Key;
            existingSpecification.Value = dto.Value;
            existingSpecification.ProductId = dto.ProductId;
            return existingSpecification;
        }

        public static IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO> MapToSpecificationDTOList(IEnumerable<Specification> specifications)
        {
            if (specifications == null) return Enumerable.Empty<Core.DTOs.SpecificationsDTOs.SpecificationDTO>();

            return specifications.Select(MapToSpecificationDTO).Where(dto => dto != null);
        }
    }
} 
