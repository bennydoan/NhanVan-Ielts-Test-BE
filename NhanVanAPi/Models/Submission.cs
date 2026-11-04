namespace NhanVanAPi.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;   // FK → AspNetUsers (Identity Id is a string)
        public int TestBankId { get; set; }                  // FK → which test
        public int RawScore { get; set; }                    // e.g. 32 / 40
        public decimal Band { get; set; }                    // e.g. 7.5
        public int TimeTakenSeconds { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public List<SubmissionAnswer> Answers { get; set; } = new(); // stored as JSON
    }
}
