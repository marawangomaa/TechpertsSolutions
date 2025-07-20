using Core.DTOs.Delivery;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Delivery> _deliveryrepo;

        public DeliveryService(IRepository<Delivery> deliveryRepo)
        {
            _deliveryrepo = deliveryRepo;
        }

        public async Task<IEnumerable<DeliveryDTO>> GetAllAsync()
        {
            var deliveries = await _deliveryrepo.GetAllAsync();
            return DeliveryMapper.MapToDeliveryDTOList(deliveries);
        }

        public async Task<DeliveryDTO?> GetByIdAsync(string id)
        {
            var delivery = await _deliveryrepo.GetByIdAsync(id);
            return DeliveryMapper.MapToDeliveryDTO(delivery);
        }

        public async Task<DeliveryDTO> AddAsync()
        {
            var entity = DeliveryMapper.MapToDelivery();

            await _deliveryrepo.AddAsync(entity);
            await _deliveryrepo.SaveChangesAsync();

            return DeliveryMapper.MapToDeliveryDTO(entity);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _deliveryrepo.GetByIdAsync(id);
            if (entity == null)
                return false;

            _deliveryrepo.Remove(entity);
            await _deliveryrepo.SaveChangesAsync();
            return true;
        }

        public async Task<DeliveryDetailsDTO?> GetDetailsByIdAsync(string id)
        {
            var entity = await _deliveryrepo.GetByIdAsync(id);
            return DeliveryMapper.MapToDeliveryDetailsDTO(entity);
        }

       
    }
}
    
