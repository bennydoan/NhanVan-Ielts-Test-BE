namespace NhanVanAPi.DTOs
{
    public class SubmissionResultDto
    {
        public int SubmissionId { get; set; }
        public int TestId { get; set; }
        public int RawScore { get; set; }
        public decimal Band { get; set; }
        public int TimeTakenSeconds { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<SubmissionAnswerDto> Answers { get; set; } = new();
    }
}
