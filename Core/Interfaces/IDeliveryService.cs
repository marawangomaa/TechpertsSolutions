using Core.DTOs.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryDTO>> GetAllAsync();
        Task<DeliveryDTO?> GetByIdAsync(string id);
        Task<DeliveryDTO> AddAsync(DeliveryCreateDTO dto);
        Task<bool> UpdateAsync(string id, DeliveryCreateDTO dto); 
        Task<bool> DeleteAsync(string id);
    }
}
