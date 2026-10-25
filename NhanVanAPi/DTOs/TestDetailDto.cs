namespace NhanVanAPi.DTOs
{
    public class TestDetailDto
        //this has all the data that we need to show the test details 
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<TestSectionDto> Sections { get; set; } = new();
    }
}
