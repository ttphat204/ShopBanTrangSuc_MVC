using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoppping_Jewelry.Repository.Validation;

namespace Shoppping_Jewelry.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống tên slider")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống mô tả")]
        public string Description { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }

        public int Status { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
