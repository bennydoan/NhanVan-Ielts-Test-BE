namespace NhanVanAPi.Models
{
    public class QuestionGroup
    {
        public int Id { get; set; }
        public int TestSectionId { get; set; }            // foreign key → which section
        public string Label { get; set; } = string.Empty; // "Questions 1 - 5"
        public int Order { get; set; }                    // 1, 2, ... inside the section

        public QuestionType Type { get; set; }
        public string? Instructions { get; set; }
        public string? Context { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public List<Question> Questions { get; set; } = new(); // one group → many questions

        public List<PassagePart> Passages { get; set; } = new();  // stored as JSON
        public List<string> SharedOptions { get; set; } = new(); // stored as JSON automatically , for drag and drop , choose correct answer 
    }
}
