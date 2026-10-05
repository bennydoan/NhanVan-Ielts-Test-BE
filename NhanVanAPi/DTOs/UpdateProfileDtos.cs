using System.ComponentModel.DataAnnotations;

namespace NhanVanAPi.DTOs
{
    public class UpdateProfileDtos
    {
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must be 10 digits and start with 0.")]
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }


    }
}
