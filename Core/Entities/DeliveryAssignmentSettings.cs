using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class DeliveryAssignmentSettings
    {
        public int MaxRetries { get; set; } = 5;
        public double PricePerKm { get; set; } = 5.0;
        public int MaxOffersPerDriver { get; set; } = 10;
        public TimeSpan OfferExpiryTime { get; set; } = TimeSpan.FromMinutes(10);
        public bool AssignNearestDriverFirst { get; set; } = true;
        public bool AllowMultipleDriversPerCluster { get; set; } = false;
        public double MaxDriverDistanceKm { get; set; } = 50;

        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(2);

        public bool EnableReassignment { get; set; } = true;

        public double CheckIntervalSeconds => CheckInterval.TotalSeconds;
        public double RetryDelaySeconds => RetryDelay.TotalSeconds;
    }
}
