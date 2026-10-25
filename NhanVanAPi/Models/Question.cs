namespace NhanVanAPi.Models
{
    public class Question
    {
        public int Id { get; set; }                       // primary key (database id)
        public int QuestionGroupId { get; set; }          // foreign key → which group
        public int QuestionNumber { get; set; }                   // question number shown to user: 1–40
        public string? Prompt { get; set; }             
        public List<string> Options { get; set; } = new();        // multiple-choice options → JSON
        public List<PassagePart> Passage { get; set; } = new();   // "Class of licence: ___" → JSON
        public List<string> CorrectAnswers { get; set; } = new(); // → JSON, can have several accepted answers
    }
}
