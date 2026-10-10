using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NhanVanAPi.DTOs;
using NhanVanAPi.Models;
using System.Security.Claims;

namespace NhanVanAPi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // find the logged-in user using the Id stored in the JWT (NameIdentifier claim)
        private async Task<ApplicationUser?> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId is null ? null : await _userManager.FindByIdAsync(userId);
        }

        // get user using Token 
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetProfile ()
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();
            return Ok(new
            {
                user.Email,
                user.FullName,
                user.PhoneNumber,
                user.DateOfBirth,
                user.AvatarUrl
            });
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDtos dto)
        {
            var user = await GetCurrentUser();

            if (user is null) return Unauthorized();

            if (dto.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest("Date of birth cannot be in the future.");
            }

            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
            }
            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword (ChangePasswordDto dto)
        {
            var user = await GetCurrentUser ();
            if (user is null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword,dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join("", result.Errors.Select(e => e.Description)));
            }
            return Ok(new { message = "Password changed successfully."});
        }

        // upload avatar endpoint 
        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromServices] IWebHostEnvironment env) // IWebHost helps us to get the physical location of wwwroot
        {
            var user = await GetCurrentUser();
            if (user is null) return Unauthorized();

            // 1. validate: never trust the file the user sends
            if (file is null || file.Length == 0) return BadRequest("No file uploaded.");
            if (file.Length > 2 * 1024 * 1024) return BadRequest("Image must be smaller than 2MB."); //protects your server from users uploading huge files.

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" ,".svg"};
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return BadRequest("Only JPG, SVG, PNG or WEBP images are allowed."); // must be these file 

            // 2. new random file name (don't use the user's file name: it could clash or contain "../"). Avoiding collision, should be unique per user 
            var fileName = $"{Guid.NewGuid()}{ext}"; //8f4c8d52-9a8e-4a4c-b2b9-6d5c2a1f4abc.jpg
            var folder = Path.Combine(env.WebRootPath, "avatars"); // C:\Users\benny\OneDrive\Desktop\NhanVan\NhanVan-Ielts-Test-BE\NhanVanApi + "avatars"
            Directory.CreateDirectory(folder); // if folder has not been created -> create on 

            using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream); // file would be saved to computer server -> change when we deploy
            }

            // 3. delete the old avatar file so the folder doesn't fill up
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var oldPath = Path.Combine(env.WebRootPath, user.AvatarUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            // 4. save the path in the database
            user.AvatarUrl = $"/avatars/{fileName}";
            await _userManager.UpdateAsync(user);

            return Ok(new { avatarUrl = user.AvatarUrl });
        }
    }
}
