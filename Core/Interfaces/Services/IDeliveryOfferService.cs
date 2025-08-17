using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IDeliveryOfferService
    {
        Task AddAsync(DeliveryOffer offer);
        Task UpdateAsync(DeliveryOffer offer);
        Task<IEnumerable<DeliveryOffer>> GetPendingOffersAsync();
        Task<IEnumerable<DeliveryOffer>> GetPendingOffersByDeliveryAsync(string deliveryId);
        Task SaveChangesAsync();
    }
}
