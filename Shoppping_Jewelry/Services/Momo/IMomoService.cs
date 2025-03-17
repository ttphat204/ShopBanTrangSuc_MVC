using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Models.Momo;

namespace Shoppping_Jewelry.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
