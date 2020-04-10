using System;
using System.Collections.Generic;
using System.Text;

namespace CharCode.ZarinPalPaymentService.Classes
{
    public class PaymentRequestConfig
    {
        public int Amount { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
