using System.ComponentModel.DataAnnotations;

namespace Shoppping_Jewelry.Models
{
    public class ProductQuantityModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống số lượng")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
