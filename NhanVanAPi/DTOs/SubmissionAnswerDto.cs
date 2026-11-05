namespace NhanVanAPi.DTOs
{
    public class SubmissionAnswerDto
    {
        public int QuestionNumber { get; set; }
        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public List<string> CorrectAnswers { get; set; } = new();

    }
}
