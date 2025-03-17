using System.ComponentModel.DataAnnotations;

namespace Shoppping_Jewelry.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetails { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên phản hồi của bạn")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email của bạn")]

        public string Email { get; set; }
    }
}
