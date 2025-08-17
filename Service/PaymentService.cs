using Core.Entities;
using Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        public PaymentService(IOptions<StripeSettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(long amount, string currency)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount * 100,
                    Currency = currency,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                };

                var service = new PaymentIntentService();
                var paymentIntent =await service.CreateAsync(options);

                return paymentIntent.ClientSecret;
            }
            catch (StripeException ex)
            {
                throw new ApplicationException($"Stripe error: {ex.Message}", ex);
            }
        }

    }
}

