using NhanVanAPi.Models;

namespace NhanVanAPi.DTOs
{
    public class QuestionGroupDto
    {
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;   // enum sent as text: "FillInTheBlank"
        public string? Instructions { get; set; }
        public string? Context { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public List<PassagePart> Passage { get; set; } = new();
        public List<string> SharedOptions { get; set; } = new();
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
