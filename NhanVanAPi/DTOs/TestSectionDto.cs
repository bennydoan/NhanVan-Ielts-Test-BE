namespace NhanVanAPi.DTOs
{
    public class TestSectionDto
    {
        public int Id { get; set; }
        public int SectionNumber { get; set; }
        public string? AudioUrl { get; set; }
        public string? PassageText { get; set; }
        public List<QuestionGroupDto> Groups { get; set; } = new();

    }
}
