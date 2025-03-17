using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoppping_Jewelry.Models;

namespace Shoppping_Jewelry.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<MomoInfoModel> MomoInfos { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ShippingModel> Shippings { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<ContactModel> Contact { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<WhistListModel> WhistLists { get; set; }
        public DbSet<CompareModel> Compares { get; set; }
        public DbSet<StatisticalModel> Statics { get; set; }
        public DbSet<ProductQuantityModel> productQuantities { get; set; }
    }
}
