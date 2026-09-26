using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NhanVanAPi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("homePage")]
        public IActionResult GetAdminPage()
        {
            return Ok(new { message = "Welcome, Admin!" });
        }
    }
}
