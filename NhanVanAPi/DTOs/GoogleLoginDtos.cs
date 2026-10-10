using System.ComponentModel.DataAnnotations;

namespace NhanVanAPi.DTOs
{
    public class GoogleLoginDtos
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
