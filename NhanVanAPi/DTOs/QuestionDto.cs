using NhanVanAPi.Models;

namespace NhanVanAPi.DTOs
{
    public class QuestionDto
    {
        public int QuestionNumber { get; set; }
        public string? Prompt { get; set; }
        public List<string> Options { get; set; } = new();
        public List<PassagePart> Passage { get; set; } = new();
    }
}
