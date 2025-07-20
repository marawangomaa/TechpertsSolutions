using Core.DTOs.Delivery;
using TechpertsSolutions.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IDeliveryService
    {
        Task<GeneralResponse<IEnumerable<DeliveryDTO>>> GetAllAsync();
        Task<GeneralResponse<DeliveryDTO>> GetByIdAsync(string id);
        Task<GeneralResponse<DeliveryDetailsDTO>> GetDetailsByIdAsync(string id); 
        Task<GeneralResponse<DeliveryDTO>> AddAsync();                              
        Task<GeneralResponse<bool>> DeleteAsync(string id);
    }
}
