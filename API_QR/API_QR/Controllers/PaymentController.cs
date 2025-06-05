using API_QR.Helpers;
using API_QR.Models;
using API_QR.Models.VnPayModels;
using API_QR.Service;
using API_QR.Service.Momo;
using Microsoft.AspNetCore.Mvc;

namespace API_QR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
       
            private readonly IVnPayService _vnPayService;
            private readonly IMomoService _momoService;
            public PaymentController(IVnPayService vnPayService, IMomoService momoService)
            {

                _vnPayService = vnPayService;
                _momoService = momoService;
            }


        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid payment information.");
            }

            try
            {
                var response = await _momoService.CreatePaymentMomo(model);
                return Ok(new { payUrl = response.PayUrl });


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult PaymentExecute()
        {
            try
            {
                var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
                return View(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("CreatePaymentUrlVnpay")]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { PaymentUrl = paymentUrl });
        }
        [HttpGet("PaymentBackReturnUrl")]
        public IActionResult PaymentBackReturnUrl()
        {

            var response = _vnPayService.PaymentExecute(Request.Query);
            return Json(response);
        }






    }
}
