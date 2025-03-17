using Microsoft.AspNetCore.Mvc;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Services.Momo;

namespace Shoppping_Jewelry.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        public PaymentController(IMomoService momoService)
        {
            _momoService = momoService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            try
            {
                var response = await _momoService.CreatePaymentMomo(model);
                if (response == null || response.ErrorCode != 0 || string.IsNullOrEmpty(response.PayUrl))
                {
                    // Trả về thông báo lỗi từ MoMo hoặc thông báo mặc định
                    string errorMessage = response?.Message ?? "Không thể tạo URL thanh toán.";
                    return BadRequest(errorMessage);
                }
                return Redirect(response.PayUrl);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi bất ngờ (lỗi mạng, deserialize, v.v.)
                return StatusCode(500, "Lỗi khi tạo thanh toán: " + ex.Message);
            }
        }

    }
}
