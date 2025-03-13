using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;
using Shoppping_Jewelry.Repository;


//Hiển thị toàn bộ sản phẩm theo thương hiệu...

namespace Shoppping_Jewelry.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            //Get thông tin Brands từ trong file dataContext 
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();

            if (brand == null) return RedirectToAction("Index");

            var productsByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);

            return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
        }

    }
}
