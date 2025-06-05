using API_QR.Models.VnPayModels;


namespace API_QR.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
