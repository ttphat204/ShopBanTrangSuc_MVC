using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shoppping_Jewelry.Repository.Validation;

namespace Shoppping_Jewelry.Models
{
    public class ContactModel
    {
        [Key]
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your Map")]
        public string Map { get; set; }

        [Required(ErrorMessage = "Please enter your Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter your description")]
        public string Description { get; set; }

        public string LogoImage { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

    }
}
