using API_QR.Models;
using API_QR.Models.MomoModels;

namespace API_QR.Service.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collections);
    }
}
