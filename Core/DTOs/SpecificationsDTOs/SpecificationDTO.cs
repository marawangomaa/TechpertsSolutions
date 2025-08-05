using Core.DTOs.ProductDTOs;

namespace Core.DTOs.SpecificationsDTOs
{
    public class SpecificationDTO
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Id { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string Key { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string Value { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string ProductId { get; set; } = string.Empty;

        public string? ProductName { get; set; }
    }
}
