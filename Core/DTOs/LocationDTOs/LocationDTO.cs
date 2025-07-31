using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.LocationDTOs
{
    public class LocationDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
    }

    public class NearestTechCompanyDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInKm { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class LocationSearchDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? MaxDistanceInKm { get; set; }
        public int? MaxResults { get; set; }
        public string? ServiceType { get; set; }
    }
} 