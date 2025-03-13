using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Repository;


//Hiển thị toàn bộ sản phẩm theo từng danh mục...

namespace Shoppping_Jewelry.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();

            if (category == null) return RedirectToAction("Index");

            var productsByCategory = _dataContext.Products.Where(p => p.Category.Id == category.Id);

            return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
