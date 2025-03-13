using System.ComponentModel.DataAnnotations;

namespace Shoppping_Jewelry.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập UserName của bạn")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email của bạn"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu của bạn")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^0[0-9]{9,10}$|^(\+84)[0-9]{9,10}$",
        ErrorMessage = "Số điện thoại phải là số hợp lệ của Việt Nam (10-11 số, bắt đầu bằng 0 hoặc +84)")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string PhoneNumber { get; set; }
    }
}
