using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanVanAPi.Data;
using NhanVanAPi.DTOs;
using NhanVanAPi.Models;

namespace NhanVanAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TestController(AppDbContext db) {
            _db = db;
        }

        // get all published test

        [HttpGet("get-test")]
        public async Task<ActionResult<List<TestBankDto>>> GetTest([FromQuery] TestCategory? category)
        {
            //we get the tests that have Ispublished = true 
            var getTest = _db.TestBank.Where(t => t.IsPublished == true);
            if (category != null)
            {
                getTest = getTest.Where(t => t.Category == category);  // ✅ adds to the existing filter

            }

            var tests = await getTest.OrderByDescending(t => t.Id).Select(t => new TestBankDto
            {
                // only select these entities 
                Id = t.Id,
                Title = t.Title,
                DurationMinutes = t.DurationMinutes,
                Category = t.Category.ToString() // convert enum to string 
            }).ToListAsync();
            return Ok(tests);
        }
        [AllowAnonymous]
        [HttpGet("get-test-byid/{id:int}")]
        public async Task<ActionResult<TestBankDto>> GetTestByID(int id)
        {
            var test = await _db.TestBank.Where(t => t.Id == id && t.IsPublished == true).
                Select(t => new TestBankDto
                {
                    Id =t.Id,
                    Title=t.Title,
                    DurationMinutes=t.DurationMinutes,
                    Category =t.Category.ToString()
                }).FirstOrDefaultAsync();

            if(test == null)
            {
                return NotFound("Test can not be found");
            }

            return Ok(test);
        }
    }
}
