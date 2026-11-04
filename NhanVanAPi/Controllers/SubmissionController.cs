using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanVanAPi.Data;
using NhanVanAPi.Services;
using NhanVanAPi.DTOs;
using NhanVanAPi.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace NhanVanAPi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController: ControllerBase
    {
        //get user first to make sure they have logged in 
        public readonly AppDbContext _db;

        //Constructor 
        public SubmissionController(AppDbContext db)
        {
            _db = db;   
        }

        private static SubmissionResultDto ToDto(Submission s) => new() // new () return a new SubmissionResultDto
        {
            SubmissionId = s.Id,
            TestId = s.TestBankId,
            RawScore = s.RawScore,
            Band = s.Band,
            TimeTakenSeconds = s.TimeTakenSeconds,
            SubmittedAt = s.SubmittedAt,
            Answers = s.Answers.Select(a => new SubmissionAnswerDto
            {
                QuestionNumber = a.QuestionNumber,
                UserAnswer = a.UserAnswer,
                IsCorrect = a.IsCorrect
            }).ToList()
        };

        //submit test endpoint
        [HttpPost("submit")]
        public async Task<ActionResult<SubmissionAnswerDto>> SubmitTest (SubmitTestDto dto)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier); // this is the user ID
            if (userID == null)  return Unauthorized();

            //getTest with correct answer 

            var test = await _db.TestBank.AsNoTracking().Include(s => s.Sections).ThenInclude(g => g.Groups).ThenInclude(q => q.Questions).FirstOrDefaultAsync(t => t.Id == dto.TestId && t.IsPublished);
            if (test == null) return NotFound("Can not find test!");

            // to collect all questions from every section and every group in a test, sort them by question number, and convert them into a list

            var questions =  test.Sections.SelectMany(s=>s.Groups).SelectMany(g => g.Questions).OrderBy(q=>q.QuestionNumber);

            // grade each answer 

            var answers = questions.Select(q =>
            {
                dto.Answers.TryGetValue(q.QuestionNumber, out var answer); // key value pair 

                var lowerKeyAnswer = answer?.Trim().ToLower();

                bool isCorrect = !string.IsNullOrEmpty(lowerKeyAnswer) && q.CorrectAnswers.Any(c => c.Trim().ToLower() == lowerKeyAnswer);

                return new SubmissionAnswer                               
                {
                    QuestionNumber = q.QuestionNumber,
                    UserAnswer = answer,
                    IsCorrect = isCorrect
                };
            }).ToList();
        

        var rawScore = answers.Count(a=>a.IsCorrect);
            
            var submission = new Submission
            {
                UserId = userID,
                TestBankId = dto.TestId,
                RawScore = rawScore,
                Band = GetBandScoreService.GetBand(rawScore),
                TimeTakenSeconds = dto.TimeTakenSeconds,
                Answers = answers,
            };

            _db.Submissions.Add(submission);

            await _db.SaveChangesAsync();          // after this, submission.Id is filled in

            return Ok(ToDto(submission));
        }

        // get TestHistory
        [HttpGet("history/{testId:int}")]
        public async Task<ActionResult<List<GetSubmissonHistoryDTO>>> GetSubmissionHistory(int testId)
        {
            // must loggin first 
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null) return Unauthorized();

            var submissionHistory = await _db.Submissions
                .Where(s => s.UserId == user && s.TestBankId == testId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new GetSubmissonHistoryDTO
                {
                    SubmissionId = s.Id,
                    Band = s.Band,
                    SubmittedAt = s.SubmittedAt,
                    TimeTakenSeconds = s.TimeTakenSeconds,
                })
                .ToListAsync();

            return Ok(submissionHistory);
        }



    }
}
