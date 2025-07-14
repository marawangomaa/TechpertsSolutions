using Core.DTOs.Delivery;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DeliveryService :IDeliveryService
    {
        private readonly IRepository<Delivery> _deliveryrepo;

        public DeliveryService(IRepository<Delivery> deliveryRepo)
        {
            _deliveryrepo = deliveryRepo;
        }

        public async Task<IEnumerable<DeliveryDTO>> GetAllAsync()
        {
            var deliveries = await _deliveryrepo.GetAllAsync();
            return deliveries.Select(MapToDTO);
        }

        public async Task<DeliveryDTO?> GetByIdAsync(string id)
        {
            var delivery = await _deliveryrepo.GetByIdAsync(id);
            return delivery == null ? null : MapToDTO(delivery);
        }

        public async Task AddAsync(DeliveryCreateDTO dto)
        {
            var entity = new Delivery
            {
                Id = Guid.NewGuid().ToString(),
                Orders = dto.Orders,
                Customers = dto.Customers,
                TechCompanies = dto.TechCompanies
            };

            await _deliveryrepo.AddAsync(entity);
            await _deliveryrepo.SaveChanges();
        }

        public async Task UpdateAsync(string id, DeliveryCreateDTO dto)
        {
            var entity = await _deliveryrepo.GetByIdAsync(id);
            if (entity == null) throw new Exception("Delivery not found");

            entity.Orders = dto.Orders;
            entity.Customers = dto.Customers;
            entity.TechCompanies = dto.TechCompanies;

            _deliveryrepo.Update(entity);
            await _deliveryrepo.SaveChanges();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _deliveryrepo.GetByIdAsync(id);
            if (entity == null) throw new Exception("Delivery not found");

            _deliveryrepo.Remove(entity);
            await _deliveryrepo.SaveChanges();
        }

        private DeliveryDTO MapToDTO(Delivery entity)
        {
            return new DeliveryDTO
            {
                Id = entity.Id,
                Orders = entity.Orders,
                Customers = entity.Customers,
                TechCompanies = entity.TechCompanies
            };
        }
    }
}
