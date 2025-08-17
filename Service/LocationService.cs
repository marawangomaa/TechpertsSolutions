using Core.DTOs.LocationDTOs;
using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using TechpertsSolutions.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class LocationService: ILocationService
    {
        private const double EarthRadiusKm = 6371;
        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        public (double Lat, double Lon) GetMidpoint(double lat1, double lon1, double lat2, double lon2) 
            => ((lat1 + lat2) / 2, (lon1 + lon2) / 2);
        
        public double CompanyToCustomer(double companyLat, double companyLon, double customerLat, double customerLon)
            => CalculateDistance(companyLat, companyLon, customerLat, customerLon);

        public double CompanyToDriver(double companyLat, double companyLon, double driverLat, double driverLon)
            => CalculateDistance(companyLat, companyLon, driverLat, driverLon);

        public double DriverToCustomer(double driverLat, double driverLon, double customerLat, double customerLon)
            => CalculateDistance(driverLat, driverLon, customerLat, customerLon);

        public double DriverToCompany(double driverLat, double driverLon, double companyLat, double companyLon)
            => CalculateDistance(driverLat, driverLon, companyLat, companyLon);
    }
}