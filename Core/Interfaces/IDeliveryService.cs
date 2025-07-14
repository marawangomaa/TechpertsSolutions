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
        Task AddAsync(DeliveryCreateDTO dto);
        Task UpdateAsync(string id, DeliveryCreateDTO dto);
        Task DeleteAsync(string id);
    }
}
