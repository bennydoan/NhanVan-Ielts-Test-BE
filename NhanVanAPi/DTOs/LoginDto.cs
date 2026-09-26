using System.ComponentModel.DataAnnotations;

namespace NhanVanAPi.DTOs
{
    //data need for logging 
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
