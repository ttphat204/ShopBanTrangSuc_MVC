using System.ComponentModel.DataAnnotations;

namespace Shoppping_Jewelry.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập UserName của bạn")]
        public string UserName { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu của bạn")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
