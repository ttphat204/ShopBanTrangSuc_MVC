using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Models.ViewModels;
using Shoppping_Jewelry.Repository;

namespace Shoppping_Jewelry.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(String searchTerm)
        {
            var products = await _dataContext.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToListAsync();
            ViewBag.KeyWord = searchTerm;
            return View(products);
        }

        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return RedirectToAction("Index");
            var productsById = _dataContext.Products.Include(p => p.Ratings)
                .Where(p => p.Id == Id).FirstOrDefault();

            //Sản phẩm liên quan
            var Relative = await _dataContext.Products
                .Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.Relative = Relative;

            var ViewModel = new ProductDetailsViewModel
            {
                ProductDetails = productsById,

            };

            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem đã có bản ghi với ProductId này chưa
                var existingRating = await _dataContext.Ratings
                    .FirstOrDefaultAsync(r => r.ProductId == rating.ProductId);

                if (existingRating != null)
                {
                    // Nếu đã có, cập nhật thông tin
                    existingRating.Name = rating.Name;
                    existingRating.Email = rating.Email;
                    existingRating.Comment = rating.Comment;
                    existingRating.Star = rating.Star;
                    TempData["success"] = "Cảm ơn bạn đã cập nhật phản hồi!";
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    var newRating = new RatingModel
                    {
                        ProductId = rating.ProductId,
                        Name = rating.Name,
                        Email = rating.Email,
                        Comment = rating.Comment,
                        Star = rating.Star
                    };
                    _dataContext.Ratings.Add(newRating);
                    TempData["success"] = "Cảm ơn bạn đã gửi phản hồi!";
                }

                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Details", new { Id = rating.ProductId });
            }
            else
            {
                TempData["error"] = "Vui lòng nhập đầy đủ thông tin";
                List<string> errors = new List<string>();
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }
                TempData["errorDetails"] = string.Join("; ", errors);
                return RedirectToAction("Details", new { Id = rating.ProductId });
            }
        }
    }
}
