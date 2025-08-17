using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IDeliveryPersonService
    {
        Task<GeneralResponse<DeliveryPersonReadDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAllAsync();
        Task<GeneralResponse<DeliveryPersonReadDTO>> UpdateAsync(string id, DeliveryPersonUpdateDTO dto);
        Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAvailableDeliveryPersonsAsync();
        Task<GeneralResponse<IEnumerable<DeliveryOfferDTO>>> GetAllOffersAsync(string driverId);
        Task<GeneralResponse<IEnumerable<DeliveryOfferDTO>>> GetPendingOffersAsync(string driverId);
        Task<GeneralResponse<bool>> AcceptOfferAsync(string offerId, string driverId);
        Task<GeneralResponse<bool>> DeclineOfferAsync(string offerId, string driverId);
        Task<GeneralResponse<bool>> CancelOfferAsync(string offerId, string driverId);
    }
} 
