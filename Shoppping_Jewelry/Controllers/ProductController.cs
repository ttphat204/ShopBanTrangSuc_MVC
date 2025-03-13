using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var productsById = _dataContext.Products.Where(p => p.Id == Id).FirstOrDefault();

            //Sản phẩm liên quan
            var Relative = await _dataContext.Products
                .Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.Relative = Relative;

            return View(productsById);
        }
    }
}
