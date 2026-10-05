namespace NhanVanAPi.DTOs;

public class AuthResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber {  get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }

    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}