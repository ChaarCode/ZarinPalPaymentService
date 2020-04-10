using System;
using System.Collections.Generic;
using System.Text;

namespace CharCode.ZarinPalPaymentService.Classes
{
    public class PaymentResponseResult
    {
        public bool IsSuccess { get; set; }

        public int ResponseCode { get; set; }
    }
}
