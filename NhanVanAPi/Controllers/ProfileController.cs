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
                user.DateOfBirth
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

    }
}
