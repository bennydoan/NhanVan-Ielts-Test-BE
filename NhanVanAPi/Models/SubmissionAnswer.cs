namespace NhanVanAPi.Models
{
    public class SubmissionAnswer
    {
        public int QuestionNumber { get; set; }
        public string? UserAnswer { get; set; }   // null = not answered
        public bool IsCorrect { get; set; }
    }
}
