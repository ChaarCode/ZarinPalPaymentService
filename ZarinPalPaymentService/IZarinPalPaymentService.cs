using CharCode.ZarinPalPaymentService.Classes;
using System.Threading.Tasks;

namespace CharCode.ZarinPalPaymentService
{
    public interface IZarinPalPaymentService
    {
        Task<PaymentRequestResult> PaymentRequestAsync(PaymentRequestConfig requestConfig);
        Task<PaymentResponseResult> PaymentResponseAsync(PaymentResponseConfig responseConfig);
    }
}