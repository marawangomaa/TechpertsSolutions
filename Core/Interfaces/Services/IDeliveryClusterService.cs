using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Interfaces.Services
{
    //public interface IDeliveryClusterService
    //{
    //    Task<GeneralResponse<DeliveryClusterDTO>> CreateClusterAsync(string deliveryId, DeliveryClusterCreateDTO dto);

    //    Task<GeneralResponse<DeliveryClusterDTO>> GetByIdAsync(string clusterId);

    //    Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetByDeliveryIdAsync(string deliveryId);

    //    Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterAsync(string clusterId, DeliveryClusterDTO dto);

    //    Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterTrackingAsync(string clusterId, DeliveryClusterTrackingDTO trackingDto);

    //    Task<GeneralResponse<bool>> DeleteClusterAsync(string clusterId);

    //    Task<GeneralResponse<DeliveryClusterTrackingDTO>> GetTrackingAsync(string clusterId);

    //    Task<GeneralResponse<DeliveryClusterDTO>> AssignDriverAsync(string clusterId, string driverId);

    //    Task<GeneralResponse<DeliveryClusterDTO>> SplitClusterAsync(Delivery delivery, DeliveryClusterDTO cluster, DeliveryPersonReadDTO driver);

    //    Task<GeneralResponse<int>> BulkAssignDriverAsync(IEnumerable<string> clusterIds, string driverId);

    //    Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetUnassignedClustersAsync();
    //}

    public interface IDeliveryClusterService
    {
        Task<GeneralResponse<DeliveryClusterDTO>> CreateClusterAsync(string deliveryId, DeliveryClusterCreateDTO dto);
        
        Task<GeneralResponse<DeliveryClusterDTO>> GetByIdAsync(string clusterId);
        
        Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetByDeliveryIdAsync(string deliveryId);
        
        Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterAsync(string clusterId, DeliveryClusterDTO dto);
        
        Task<GeneralResponse<bool>> DeleteClusterAsync(string clusterId);

        Task<GeneralResponse<DeliveryClusterDTO>> AssignDriverAsync(string clusterId, string driverId);
        
        Task AutoAssignDriverAsync(Delivery delivery, string clusterId, double? overrideLat = null, double? overrideLon = null);
        
        Task<GeneralResponse<DeliveryClusterDTO>> SplitClusterAsync(Delivery delivery, DeliveryClusterDTO cluster, DeliveryPersonReadDTO driver);

        Task<GeneralResponse<DeliveryClusterDTO>> GetTrackingAsync(string clusterId);
        
        Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterTrackingAsync(string clusterId, DeliveryClusterTrackingDTO trackingDto);

        Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetUnassignedClustersAsync();
    }
}
