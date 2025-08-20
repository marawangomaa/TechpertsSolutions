using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        public PaymentService(IOptions<StripeSettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(decimal amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe expects smallest currency unit (cents)
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return (intent.Id, intent.ClientSecret);
        }
        public async Task<bool> VerifyPaymentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                // Status values: requires_payment_method, requires_confirmation, succeeded, etc.
                return paymentIntent.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                throw new ApplicationException($"Stripe error: {ex.Message}", ex);
            }
        }
    }
}
