using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoppping_Jewelry.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        public string Name { get; set; }  // Sửa kiểu dữ liệu từ 'int' thành 'string'

        [Required(ErrorMessage = "Vui lòng nhập email của bạn")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }  // Sửa kiểu dữ liệu từ 'int' thành 'string'

        [Required(ErrorMessage = "Vui lòng nhập comment của bạn")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá chỉ được từ 1 đến 5 sao")]
        public int Star { get; set; }  // Sửa kiểu dữ liệu từ 'string' thành 'int' để dễ xử lý

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
