using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PaymentsDTOs
{
    public class CreatePaymentIntentRequest
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
    }
}
