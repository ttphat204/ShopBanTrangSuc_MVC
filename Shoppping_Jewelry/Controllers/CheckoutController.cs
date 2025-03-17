using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shoppping_Jewelry.Areas.Admin.Repository;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Repository;
using Shoppping_Jewelry.Services.Momo;

namespace Shoppping_Jewelry.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private readonly IMomoService _momoService;
        public CheckoutController(IEmailSender emailSender, DataContext context, IMomoService momoService)
        {
            _emailSender = emailSender;
            _dataContext = context;
            _momoService = momoService;
        }

        public IActionResult Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return View("GuestCheckout");
            }
            return ProcessOrder(userEmail).Result;
        }

        [HttpPost]
        public IActionResult GuestCheckout(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Vui lòng nhập email để tiếp tục đặt hàng.");
                return View();
            }
            return ProcessOrder(email).Result;
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInfoModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            if (requestQuery["resultCode"] != "0") // giao dịch không thành công
            {
                var newMomoInsert = new MomoInfoModel
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    DatePaid = DateTime.Now
                };

                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                TempData["success"] = "Đã hủy giao dịch MoMo.";
                return RedirectToAction("Index", "Cart");
            }



            //Mai kiểm tra mại bằng momo test
            //if (string.Equals(requestQuery["resultCode"], "0", StringComparison.OrdinalIgnoreCase))
            //{
            //    var newMomoInsert = new MomoInfoModel
            //    {
            //        OrderId = requestQuery["orderId"],
            //        FullName = User?.FindFirstValue(ClaimTypes.Email) ?? "Unknown",
            //        Amount = decimal.TryParse(requestQuery["amount"], out decimal amount) ? amount : 0,
            //        OrderInfo = requestQuery["orderInfo"],
            //        DatePaid = DateTime.Now
            //    };

            //    _dataContext.Add(newMomoInsert);
            //    await _dataContext.SaveChangesAsync();
            //}
            //else
            //{
            //    TempData["success"] = "Đã hủy giao dịch MoMo.";
            //    return RedirectToAction("Index", "Cart");
            //}




            //var checkoutResult = await Checkout(requestQuery["orderId"]);
            // return View(checkoutResult);

            return View(response);
        }



        private async Task<IActionResult> ProcessOrder(string email)
        {
            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            if (cartItems == null || !cartItems.Any())
            {
                TempData["error"] = "Giỏ hàng trống, không thể đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }
            // Lấy shipping cost từ cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            if (shippingPriceCookie != null)
            {
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceCookie);
            }

            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel
            {
                OrderCode = orderCode,
                ShippingCost = shippingPrice,
                UserName = email,
                Status = 1,
                CreateDate = DateTime.Now
            };

            _dataContext.Add(orderItem);
            _dataContext.SaveChanges();
            Console.WriteLine($"Order saved with ShippingCost: {orderItem.ShippingCost}, ID: {orderItem.Id}");
            foreach (var cart in cartItems)
            {
                var orderDetails = new OrderDetailModel();

                orderDetails.UserName = email;
                orderDetails.OrderCode = orderCode;

                orderDetails.ProductId = cart.ProductId;
                orderDetails.Price = cart.Price;
                orderDetails.Quantity = cart.Quantity;

                var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                product.Quantity -= cart.Quantity;
                product.Sold += cart.Quantity;

                _dataContext.Update(product);
                _dataContext.Add(orderDetails);
                _dataContext.SaveChanges();
            }


            HttpContext.Session.Remove("Cart");

            // Gửi email cho khách hàng
            var customerSubject = "Đặt hàng thành công";
            var customerMessage = BuildCustomerEmailMessage(orderCode);
            bool customerEmailSent = await SendEmailAsync(email, customerSubject, customerMessage);

            if (!customerEmailSent)
            {
                TempData["warning"] = "Đặt hàng thành công nhưng gửi email xác nhận cho bạn thất bại.";
            }

            // Gửi email cho admin
            var adminEmail = "trantanphat08012004@gmail.com"; // Thay bằng email admin hoặc lấy từ cấu hình
            var adminSubject = "Thông báo: Đơn hàng mới từ khách hàng";
            var adminMessage = BuildAdminEmailMessage(orderCode, email, cartItems);
            bool adminEmailSent = await SendEmailAsync(adminEmail, adminSubject, adminMessage);

            if (!adminEmailSent)
            {
                TempData["warning"] = TempData["warning"]?.ToString() + " Gửi email thông báo cho admin thất bại.";
                // Có thể ghi log lỗi ở đây
            }
            TempData["success"] = "Đặt hàng thành công, vui lòng chờ duyệt.";
            return RedirectToAction("Index", "Cart");
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                await _emailSender.SendEmailAsync(email, subject, message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string BuildCustomerEmailMessage(string orderCode)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Đặt hàng thành công!</h3>");
            sb.AppendLine($"<p>Cảm ơn bạn đã đặt hàng. Mã đơn hàng của bạn là: <strong>{orderCode}</strong>.</p>");
            sb.AppendLine("<p>Chúng tôi sẽ xử lý đơn hàng sớm nhất. Vui lòng kiểm tra email thường xuyên để cập nhật trạng thái.</p>");
            return sb.ToString();
        }

        private string BuildAdminEmailMessage(string orderCode, string customerEmail, List<CartItemModel> cartItems)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Đơn hàng mới</h3>");
            sb.AppendLine($"<p><strong>Mã đơn hàng:</strong> {orderCode}</p>");
            sb.AppendLine($"<p><strong>Email khách hàng:</strong> {customerEmail}</p>");
            sb.AppendLine($"<p><strong>Ngày đặt:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("<h4>Chi tiết đơn hàng:</h4>");
            sb.AppendLine("<table border='1'><tr><th>ID Sản phẩm</th><th>Số lượng</th><th>Giá</th><th>Thành tiền</th></tr>");

            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                var subtotal = item.Price * item.Quantity;
                totalAmount += subtotal;
                sb.AppendLine($"<tr><td>{item.ProductId}</td><td>{item.Quantity}</td><td>{item.Price:C}</td><td>{subtotal:C}</td></tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine($"<p><strong>Tổng tiền:</strong> {totalAmount:C}</p>");
            sb.AppendLine("<p>Vui lòng kiểm tra và xử lý đơn hàng trong hệ thống.</p>");

            return sb.ToString();
        }
    }
}