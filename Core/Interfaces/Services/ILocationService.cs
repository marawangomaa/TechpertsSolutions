using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface ILocationService
    {
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        (double Lat, double Lon) GetMidpoint(double lat1, double lon1, double lat2, double lon2);
        double CompanyToCustomer(double companyLat, double companyLon, double customerLat, double customerLon);
        double CompanyToDriver(double companyLat, double companyLon, double driverLat, double driverLon);
        double DriverToCustomer(double driverLat, double driverLon, double customerLat, double customerLon);
        double DriverToCompany(double driverLat, double driverLon, double companyLat, double companyLon);
    }
}
