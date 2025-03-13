using Microsoft.AspNetCore.Identity;

namespace Shoppping_Jewelry.Models
{
    public class AppUserModel : IdentityUser
    {
        public string RoleId { get; set; }
    }
}
