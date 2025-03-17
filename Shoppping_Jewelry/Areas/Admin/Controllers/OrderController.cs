using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Repository;

namespace Shoppping_Jewelry.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10; // Số đơn hàng mỗi trang
            if (pg < 1) pg = 1; // Đảm bảo trang không nhỏ hơn 1

            // Lấy danh sách đơn hàng với sắp xếp
            var ordersQuery = _dataContext.Orders
                .OrderByDescending(p => p.Id);

            // Tính tổng số đơn hàng
            int totalOrders = await ordersQuery.CountAsync();

            // Tạo đối tượng Paginate
            var pager = new Paginate(totalOrders, pg, pageSize);

            // Tính số đơn hàng cần bỏ qua và lấy dữ liệu phân trang
            int skipItems = (pg - 1) * pageSize;
            var orders = await ordersQuery
                .Skip(skipItems)
                .Take(pager.PageSize)
                .ToListAsync();

            // Gán pager vào ViewBag để truyền sang view
            ViewBag.Pager = pager;

            return View(orders);
        }

        [HttpGet]
        [Route("ViewOrder")]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderCode == ordercode)
                .ToListAsync();

            var Order = _dataContext.Orders
                .Where(o => o.OrderCode == ordercode)
                .First();

            var ShippingCost = _dataContext.Orders
                .Where(s => s.OrderCode == ordercode).First();
            ViewBag.ShippingCost = ShippingCost.ShippingCost;

            ViewBag.Status = Order.Status;
            return View(DetailsOrder);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _dataContext.Update(order);
            if (status == 2)
            {
                var DetailsOrder = await _dataContext.OrderDetails
                    .Include(od => od.Product)
                    .Where(od => od.OrderCode == order.OrderCode)
                    .Select(od => new
                    {
                        od.Quantity,
                        od.Product.Price,
                        od.Product.CapitalPrice
                    }).ToListAsync();
                var staticsModel = await _dataContext.Statics
                    .FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreateDate.Date);
                if (staticsModel != null)
                {
                    foreach (var orderDetail in DetailsOrder)
                    {
                        staticsModel.Quantity += 1;
                        staticsModel.Sold += orderDetail.Quantity;
                        staticsModel.Revenue += orderDetail.Quantity * orderDetail.Price;
                        staticsModel.Profit += orderDetail.Price - orderDetail.CapitalPrice;
                    }
                    _dataContext.Update(staticsModel);
                }
                else
                {
                    int new_quantity = 0;
                    int new_sold = 0;
                    decimal new_profit = 0;
                    foreach (var orderDetail in DetailsOrder)
                    {
                        new_quantity += 1;
                        new_sold += orderDetail.Quantity;
                        new_profit += orderDetail.Price - orderDetail.CapitalPrice;
                        staticsModel = new StatisticalModel
                        {
                            DateCreated = order.CreateDate,
                            Quantity = new_quantity,
                            Sold = new_sold,
                            Revenue = orderDetail.Quantity * orderDetail.Price,
                            Profit = new_profit
                        };
                    }
                    _dataContext.Add(staticsModel);
                }
            }
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while updating the order status.");
            }
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string ordercode)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }
            try
            {

                //delete order
                _dataContext.Orders.Remove(order);


                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Đã xóa đơn hàng";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return StatusCode(500, "An error occurred while deleting the order.");
            }

        }

    }
}