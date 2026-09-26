using Microsoft.AspNetCore.Identity;

namespace NhanVanAPi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // it is a inheritance from identity user which has Id (a string, Guid-based), Email, PasswordHash, PhoneNumber, EmailConfirmed, UserName
        public string FullName { get; set; } = string.Empty; // this one is not provided automatically by identity 

    }
}
