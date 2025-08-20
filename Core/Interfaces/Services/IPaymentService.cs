using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(decimal amount, string currency);
        Task<bool> VerifyPaymentAsync(string paymentIntentId);
    }
}