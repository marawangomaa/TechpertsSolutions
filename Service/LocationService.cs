using Core.DTOs.LocationDTOs;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<AppUser> _userRepo;

        public LocationService(
            IRepository<TechCompany> techCompanyRepo,
            IRepository<AppUser> userRepo)
        {
            _techCompanyRepo = techCompanyRepo;
            _userRepo = userRepo;
        }

        public async Task<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>> GetNearestTechCompaniesAsync(LocationSearchDTO searchDto)
        {
            try
            {
                // Optimized includes for tech company listing with user information for location services
                var techCompanies = await _techCompanyRepo.GetAllWithIncludesAsync(
                    tc => tc.User);

                var nearestCompanies = techCompanies
                    .Where(tc => tc.Latitude.HasValue && tc.Longitude.HasValue)
                    .Select(tc => new NearestTechCompanyDTO
                    {
                        Id = tc.Id,
                        Name = tc.User?.UserName ?? "Unknown Company",
                        Latitude = tc.Latitude.Value,
                        Longitude = tc.Longitude.Value,
                        Address = tc.Address ?? "",
                        DistanceInKm = CalculateDistance(
                            searchDto.Latitude, 
                            searchDto.Longitude, 
                            tc.Latitude.Value, 
                            tc.Longitude.Value)
                    })
                    .OrderBy(tc => tc.DistanceInKm)
                    .Take(searchDto.MaxResults ?? 10)
                    .ToList();

                return new GeneralResponse<IEnumerable<NearestTechCompanyDTO>>
                {
                    Success = true,
                    Message = "Nearest tech companies retrieved successfully.",
                    Data = nearestCompanies
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<NearestTechCompanyDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving nearest tech companies.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<NearestTechCompanyDTO>> GetNearestTechCompanyAsync(double latitude, double longitude, string? serviceType = null)
        {
            try
            {
                // Optimized includes for tech company listing with user information for location services
                var techCompanies = await _techCompanyRepo.GetAllWithIncludesAsync(
                    tc => tc.User);

                var nearestCompany = techCompanies
                    .Where(tc => tc.Latitude.HasValue && tc.Longitude.HasValue)
                    .Select(tc => new
                    {
                        TechCompany = tc,
                        Distance = CalculateDistance(latitude, longitude, tc.Latitude.Value, tc.Longitude.Value)
                    })
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();

                if (nearestCompany == null)
                {
                    return new GeneralResponse<NearestTechCompanyDTO>
                    {
                        Success = false,
                        Message = "No tech companies found with location data.",
                        Data = null
                    };
                }

                var result = new NearestTechCompanyDTO
                {
                    Id = nearestCompany.TechCompany.Id,
                    Name = nearestCompany.TechCompany.User?.UserName ?? "Unknown Company",
                    Latitude = nearestCompany.TechCompany.Latitude.Value,
                    Longitude = nearestCompany.TechCompany.Longitude.Value,
                    Address = nearestCompany.TechCompany.Address ?? "",
                    DistanceInKm = nearestCompany.Distance
                };

                return new GeneralResponse<NearestTechCompanyDTO>
                {
                    Success = true,
                    Message = "Nearest tech company retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<NearestTechCompanyDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the nearest tech company.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<double>> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                var distance = CalculateDistance(lat1, lon1, lat2, lon2);
                return new GeneralResponse<double>
                {
                    Success = true,
                    Message = "Distance calculated successfully.",
                    Data = distance
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<double>
                {
                    Success = false,
                    Message = $"Error calculating distance: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<GeneralResponse<LocationDTO>> GetLocationFromAddressAsync(string address)
        {
            try
            {
                // This would typically integrate with a geocoding service like Google Maps API
                // For now, we'll return a mock response
                var location = new LocationDTO
                {
                    Address = address,
                    City = "Unknown",
                    Country = "Unknown",
                    Latitude = 0,
                    Longitude = 0
                };

                return new GeneralResponse<LocationDTO>
                {
                    Success = true,
                    Message = "Location retrieved successfully.",
                    Data = location
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<LocationDTO>
                {
                    Success = false,
                    Message = $"Error retrieving location: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<LocationDTO>> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                // This would typically integrate with a reverse geocoding service
                // For now, we'll return a mock response
                var location = new LocationDTO
                {
                    Address = "Unknown Address",
                    City = "Unknown City",
                    Country = "Unknown Country",
                    Latitude = latitude,
                    Longitude = longitude
                };

                return new GeneralResponse<LocationDTO>
                {
                    Success = true,
                    Message = "Address retrieved successfully.",
                    Data = location
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<LocationDTO>
                {
                    Success = false,
                    Message = $"Error retrieving address: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateUserLocationAsync(string userId, double latitude, double longitude)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "User not found.",
                        Data = false
                    };
                }

                user.Latitude = latitude;
                user.Longitude = longitude;
                user.UpdatedAt = DateTime.UtcNow;

                _userRepo.Update(user);
                await _userRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "User location updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating user location: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateTechCompanyLocationAsync(string techCompanyId, double latitude, double longitude, string address)
        {
            try
            {
                var techCompany = await _techCompanyRepo.GetByIdAsync(techCompanyId);
                if (techCompany == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Tech company not found.",
                        Data = false
                    };
                }

                techCompany.Latitude = latitude;
                techCompany.Longitude = longitude;
                techCompany.Address = address;
                techCompany.UpdatedAt = DateTime.UtcNow;

                _techCompanyRepo.Update(techCompany);
                await _techCompanyRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Tech company location updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating tech company location: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<NearestTechCompanyDTO>>> GetTechCompaniesInRadiusAsync(double latitude, double longitude, double radiusInKm)
        {
            try
            {
                var techCompanies = await _techCompanyRepo.GetAllWithIncludesAsync(tc => tc.User);
                techCompanies = techCompanies.Where(tc => tc.IsActive && tc.Latitude.HasValue && tc.Longitude.HasValue);

                var companiesInRadius = techCompanies
                    .Select(tc => new
                    {
                        TechCompany = tc,
                        Distance = CalculateDistance(latitude, longitude, tc.Latitude.Value, tc.Longitude.Value)
                    })
                    .Where(x => x.Distance <= radiusInKm)
                    .OrderBy(x => x.Distance)
                    .Select(x => MapToNearestTechCompanyDTO(x.TechCompany, x.Distance));

                return new GeneralResponse<IEnumerable<NearestTechCompanyDTO>>
                {
                    Success = true,
                    Message = "Tech companies in radius retrieved successfully.",
                    Data = companiesInRadius
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<NearestTechCompanyDTO>>
                {
                    Success = false,
                    Message = $"Error retrieving tech companies in radius: {ex.Message}",
                    Data = null
                };
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private NearestTechCompanyDTO MapToNearestTechCompanyDTO(TechCompany techCompany, double distance)
        {
            return new NearestTechCompanyDTO
            {
                Id = techCompany.Id,
                Name = techCompany.User?.FullName ?? "Unknown",
                Address = techCompany.Address ?? techCompany.User?.Address ?? "Unknown",
                City = techCompany.City ?? techCompany.User?.City ?? "Unknown",
                Country = techCompany.Country ?? techCompany.User?.Country ?? "Unknown",
                Latitude = techCompany.Latitude ?? 0,
                Longitude = techCompany.Longitude ?? 0,
                DistanceInKm = distance,
                PhoneNumber = techCompany.PhoneNumber ?? techCompany.User?.PhoneNumber,
                Website = techCompany.Website,
                Description = techCompany.Description,
                IsActive = techCompany.IsActive,
                Rating = 0, // TODO: Implement rating system
                ReviewCount = 0 // TODO: Implement review system
            };
        }
    }
} 