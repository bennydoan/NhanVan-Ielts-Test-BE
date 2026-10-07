 using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NhanVanAPi.Data;
using NhanVanAPi.DTOs;
using NhanVanAPi.Models;
using NhanVanAPi.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace NhanVanAPi.Controllers;

// IActionresult returns a HTTP response
// promise that something will finish in the future.

[ApiController]
[Route("api/[controller]")] // define the Url 
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;

    //create constructor
    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService, EmailService emailService) { 
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;

    }

    // IActionResult allows you to return different types of HTTP responses.

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto) // this method received dto param 
    {

        //create new user 
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password); // hashed password

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); //contain characters such as +, /, or =.
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken)); // String (has specical characters) → byte array ---> Bytes → URL-safe string 

        var confirmationLink = $"http://localhost:3000/auth/confirm-email?email={dto.Email}&token={encodedToken}";
        await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

        return Ok(new { message = "Registered successfully. Waiting for Email confirmation."}
        );
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return BadRequest("Invalid confirmation link.");
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)); // convert it back to rawToken
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);// it must come with a token, otherwise, user can type in a randome email and get confirmed

        if (!result.Succeeded)
        {
            return BadRequest("Invalid or expired confirmation link.");
        }

        return Ok(new { message = "Email confirmed successfully! You can now log in." });
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        if (!user.EmailConfirmed)
        {
            return Unauthorized("Please confirm your email before logging in. Check your Email");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        // send the HttpOnly cookie,  JWT is stored into a cookie . JS cant read it because it is http only
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true, // sent Auth cookies
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt
        });

        return Ok(new
        {
            Email = user.Email,
            FullName = user.FullName,
            DOB = user.DateOfBirth,
            Phone = user.PhoneNumber,
            Role = roles.FirstOrDefault() ?? "Student",
            ExpiresAt = expiresAt
        });
    }

    // because browser cant read the HTTP Cookie, this endpoint will help to tell who loggin . Call this to verify loggin 

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            Email = User.FindFirstValue(ClaimTypes.Email),
            FullName = User.FindFirstValue(ClaimTypes.Name),
            Role = User.FindFirstValue(ClaimTypes.Role) ?? "Student"
        });
    }

    [HttpPost("logout")]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        return Ok(new { message = "Logged out successfully." });
    }
}