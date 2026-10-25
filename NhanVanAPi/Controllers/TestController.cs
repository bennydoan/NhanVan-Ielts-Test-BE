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
        public TestController(AppDbContext db)
        {
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
                    Id = t.Id,
                    Title = t.Title,
                    DurationMinutes = t.DurationMinutes,
                    Category = t.Category.ToString()
                }).FirstOrDefaultAsync();

            if (test == null)
            {
                return NotFound("Test can not be found");
            }

            return Ok(test);
        }

        

        // get one test with all its sections, groups and questions (NO answers)

        [AllowAnonymous]
        [HttpGet("get-test-content/{id:int}")]
        public async Task<ActionResult<TestDetailDto>> GetTestContent(int id)
        {
            // get test first and then get everything that linked to it 

            var test = await _db.TestBank.AsNoTracking()                              // read-only, faster
            .Include(s=>s.Sections).
            ThenInclude(g=>g.Groups).
            ThenInclude(q => q.Questions).FirstOrDefaultAsync(t=>t.Id == id && t.IsPublished == true);

            if (test == null) return NotFound("Can not find test!");

            var dto = new TestDetailDto
            {
                Id = test.Id,
                Title = test.Title,
                DurationMinutes = test.DurationMinutes,
                Category = test.Category.ToString(),
                Sections = test.Sections.OrderBy(s => s.SectionNumber).Select(s => new TestSectionDto
                {
                    Id = s.Id,
                    SectionNumber = s.SectionNumber,
                    AudioUrl = s.AudioUrl,
                    PassageText = s.PassageText,
                    Groups = s.Groups.OrderBy(g => g.Order).Select(g=> new QuestionGroupDto
                    {
                        
                        Label = g.Label,
                        Type = g.Type.ToString(),
                        Instructions = g.Instructions,
                        Context = g.Context,
                        ImageUrl = g.ImageUrl,
                        ImageAlt = g.ImageAlt,
                        Passage = g.Passages,
                        SharedOptions = g.SharedOptions,
                        Questions = g.Questions
                            .OrderBy(q => q.QuestionNumber)
                            .Select(q => new QuestionDto // no correct answers
                            {
                                QuestionNumber = q.QuestionNumber,
                                Prompt = q.Prompt,
                                Options = q.Options,
                                Passage = q.Passage
                            }).ToList()
                    }).ToList()
                }).ToList()
            };
            return Ok(dto);
        }
    }
}
