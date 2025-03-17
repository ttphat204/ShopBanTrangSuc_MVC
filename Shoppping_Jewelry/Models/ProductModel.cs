using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoppping_Jewelry.Repository.Validation;

namespace Shoppping_Jewelry.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        [MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá vốn")]
        public decimal CapitalPrice { get; set; }
        public string Slug { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập Giá Sản Phẩm")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá Sản Phẩm phải lớn hơn 0")]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Chọn một thương hiệu")]
        [Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chọn một thương hiệu")]
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Chọn một danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chọn một danh mục")]
        public int CategoryId { get; set; }

        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }

        public string Image { get; set; }

        public int Quantity { get; set; }
        public int Sold { get; set; }


        public RatingModel Ratings { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

    }
}
