using Core.DTOs.ProductDTOs;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class CompatibleProductDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string TechCompanyName { get; set; }
        public List<SpecificationDTO> Specifications { get; set; } = new List<SpecificationDTO>();
        public bool IsCompatible { get; set; }
        public string? CompatibilityNotes { get; set; }
    }

    public class CompatibilityFilterDTO
    {
        public string? CategoryId { get; set; }
        public string? ComponentId { get; set; }
        public string? SubCategoryId { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? TechCompanyId { get; set; }
    }
} 
