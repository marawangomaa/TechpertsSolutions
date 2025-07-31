using Core.DTOs.LocationDTOs;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ILocationService
    {
        Task<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>> GetNearestTechCompaniesAsync(LocationSearchDTO searchDto);
        Task<GeneralResponse<NearestTechCompanyDTO>> GetNearestTechCompanyAsync(double latitude, double longitude, string? serviceType = null);
        Task<GeneralResponse<double>> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2);
        Task<GeneralResponse<LocationDTO>> GetLocationFromAddressAsync(string address);
        Task<GeneralResponse<LocationDTO>> GetAddressFromCoordinatesAsync(double latitude, double longitude);
        Task<GeneralResponse<bool>> UpdateUserLocationAsync(string userId, double latitude, double longitude);
        Task<GeneralResponse<bool>> UpdateTechCompanyLocationAsync(string techCompanyId, double latitude, double longitude, string address);
        Task<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>> GetTechCompaniesInRadiusAsync(double latitude, double longitude, double radiusInKm);
    }
} 