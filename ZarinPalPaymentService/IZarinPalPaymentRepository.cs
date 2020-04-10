using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CharCode.ZarinPalPaymentService
{
    public interface IZarinPalPaymentRepository
    {
        /// <summary>
        /// insert payment into database and return that id.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<long> InsertAsync(int amount, string authority);

        Task SetSuccessAsync(string authority, long refId);

        Task SetFailAsync(string authority);
    }
}
