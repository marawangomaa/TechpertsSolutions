using Core.DTOs.Delivery;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
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
            return deliveries.Select(d => new DeliveryDTO { Id = d.Id });
        }

        public async Task<DeliveryDTO?> GetByIdAsync(string id)
        {
            var delivery = await _deliveryrepo.GetByIdAsync(id);
            return delivery == null ? null : new DeliveryDTO { Id = delivery.Id };
        }

        public async Task<DeliveryDTO> AddAsync()
        {
            var entity = new Delivery
            {
                Id = Guid.NewGuid().ToString()
            };

            await _deliveryrepo.AddAsync(entity);
            await _deliveryrepo.SaveChangesAsync();

            return new DeliveryDTO
            {
                Id = entity.Id
            };
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
            if (entity == null) return null;

            return new DeliveryDetailsDTO
            {
                Id = entity.Id,
                Customers = entity.Customers?.Select(c => new DeliveryCustomerDTO
                {
                    Id = c.Id,
                    City = c.City,
                    Country = c.Country,
                    UserFullName = c.User.FullName
                }).ToList(),

                Orders = entity.Orders?.Select(o => new DeliveryOrderDTO
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    CustomerName = o.Customer?.User?.FullName ?? "",
                    City = o.Customer?.City,
                    Status = o.Status
                }).ToList(),

                TechCompanies = entity.TechCompanies?.Select(t => new DeliveryTechCompanyDTO
                {
                    Id = t.Id,
                    City = t.City,
                    Country = t.Country,
                    UserFullName = t.User.FullName
                }).ToList()
            };
        }

       
    }
}
    
