using CharCode.ZarinPalPaymentService.Classes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZarinPal;

namespace CharCode.ZarinPalPaymentService
{
    public class ZarinPalPaymentService : IZarinPalPaymentService
    {
        private readonly IZarinPalPaymentRepository zarinPalPaymentRepository;
        private readonly string callbackURL;
        private readonly string merchantId;
        private const string paymentUrlBase = @"https://www.zarinpal.com/pg/StartPay/";

        public ZarinPalPaymentService(IZarinPalPaymentRepository zarinPalPaymentRepository, IConfiguration configuration)
        {
            this.zarinPalPaymentRepository = zarinPalPaymentRepository ?? throw new ArgumentNullException(nameof(zarinPalPaymentRepository));
            this.callbackURL = configuration["ZarinPal:CallbackURL"];
            this.merchantId = configuration["ZarinPal:MerchantId"];
        }

        public async Task<PaymentRequestResult> PaymentRequestAsync(PaymentRequestConfig requestConfig)
        {
            var zp = GetService();

            var response = await zp.PaymentRequestAsync(merchantId, requestConfig.Amount, requestConfig.Name, requestConfig.Email, requestConfig.PhoneNumber, callbackURL);

            await zp.CloseAsync();

            CheckStatus(response);

            var authority = response.Body.Authority;
            var paymentId = await zarinPalPaymentRepository.InsertAsync(requestConfig.Amount, authority);
            var paymentUrl = paymentUrlBase + authority;

            var result = new PaymentRequestResult()
            {
                PaymentId = paymentId,
                PaymentURL = paymentUrl
            };

            return result;
        }

        private static PaymentGatewayImplementationServicePortTypeClient GetService()
        {
            ServicePointManager.Expect100Continue = false;
            var zp = new PaymentGatewayImplementationServicePortTypeClient();
            return zp;
        }

        private static void CheckStatus(PaymentRequestResponse response)
        {
            var status = response.Body.Status;
            if (status != 100)
            {
                throw new Exception($"Something is wrong! Error code: {status}");
            }
        }

        public async Task<PaymentResponseResult> PaymentResponseAsync(PaymentResponseConfig responseConfig)
        {
            var zp = GetService();

            var authority = responseConfig.Authority;
            var amount = await zarinPalPaymentRepository.GetAmountByAuthorityAsync(authority);
            var response = await zp.PaymentVerificationAsync(merchantId, authority, amount);

            await zp.CloseAsync();

            var isSuccess = false;
            var status = response.Body.Status;

            if (IsSuccess(status))
            {
                await SetSuccessAsync(authority, response);
                isSuccess = true;
            }
            else
            {
                await SetFailAsync(authority);
            }

            var result = new PaymentResponseResult() { IsSuccess = isSuccess, ResponseCode = status };

            return result;
        }

        private async Task SetSuccessAsync(string authority, PaymentVerificationResponse response)
        {
            long refID = response.Body.RefID;
            await zarinPalPaymentRepository.SetSuccessAsync(authority, refID);
        }

        private async Task SetFailAsync(string authority)
        {
            await zarinPalPaymentRepository.SetFailAsync(authority);
        }

        private static bool IsSuccess(int status)
        {
            return status == 100 || status == 101;
        }
    }
}
