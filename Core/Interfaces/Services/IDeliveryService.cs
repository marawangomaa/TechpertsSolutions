using Core.DTOs.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryDTO>> GetAllAsync();
        Task<DeliveryDTO?> GetByIdAsync(string id);
        Task<DeliveryDetailsDTO?> GetDetailsByIdAsync(string id); 
        Task<DeliveryDTO> AddAsync();                              
        Task<bool> DeleteAsync(string id);
    }
}
