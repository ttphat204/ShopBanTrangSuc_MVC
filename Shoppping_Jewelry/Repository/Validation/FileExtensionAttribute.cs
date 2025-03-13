using System.ComponentModel.DataAnnotations;

namespace Shoppping_Jewelry.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png", "jpeg" };

                bool result = extensions.Any(x => extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Chỉ chấp nhận hình dưới dạng: png, jpg, jpeg");
                }

            }
            return ValidationResult.Success;
        }
    }
}
