using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;

namespace Shoppping_Jewelry.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context, ILogger<SeedData> logger)
        {
            try
            {
                _context.Database.Migrate();
                if (!_context.Products.Any())
                {
                    CategoryModel Ring = new CategoryModel { Name = "NhanCuoi", Slug = "nhancuoi", Description = "PNJ thương hiệu nổi tiếng", Status = 1 };
                    CategoryModel Necklack = new CategoryModel { Name = "Vongco", Slug = "vongco", Description = "SJC thương hiệu nổi tiếng", Status = 1 };
                    BrandModel PNJ = new BrandModel { Name = "PNJ", Slug = "nhancuoi", Description = "PNJ thương hiệu nổi tiếng", Status = 1 };
                    BrandModel SJC = new BrandModel { Name = "SJC", Slug = "vongco", Description = "SJC thương hiệu nổi tiếng", Status = 1 };
                    _context.Products.AddRange(
                        new ProductModel { Name = "Nhẫn hột soàn", Slug = "Nhan hot soan", Description = "Nhẫn hột soàn hiệu PNJ", Image = "1.jpg", Category = Ring, Brand = PNJ, Price = 143 },
                        new ProductModel { Name = "Vòng cổ kim cương", Slug = "vong co kim cuong", Description = "Vòng cổ thương hiệu SJC", Image = "1.jpg", Category = Necklack, Brand = SJC, Price = 153 }
                    );
                    _context.SaveChanges();
                    logger.LogInformation("Dữ liệu đã được thêm thành công.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Đã xảy ra lỗi khi thêm dữ liệu.");
            }
        }
    }
}
