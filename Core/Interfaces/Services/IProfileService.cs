using Core.DTOs;
using Core.DTOs.ProfileDTOs;

namespace Core.Interfaces.Services
{
    public interface IProfileService
    {
        Task<GeneralResponse<IEnumerable<GeneralProfileReadDTO>>> GetAllProfilesAsync();
        Task<GeneralResponse<GeneralProfileReadDTO>> GetProfileByIdAsync(string userId);
        Task<GeneralResponse<CustomerProfileDTO>> GetCustomerRelatedInfoAsync(string userId);
        Task<GeneralResponse<TechCompanyProfileDTO>> GetTechCompanyRelatedInfoAsync(string userId);
        Task<GeneralResponse<DeliveryPersonProfileDTO>> GetDeliveryPersonRelatedInfoAsync(string userId);

    }
}
