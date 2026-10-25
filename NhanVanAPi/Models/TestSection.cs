namespace NhanVanAPi.Models
{
    public class TestSection
    {
        public int Id { get; set; }                  // primary key
        public int TestBankId { get; set; }          // foreign key,  which test this section belongs to
        public int SectionNumber { get; set; }       // 1–4
        public string? AudioUrl { get; set; }        // Listening only
        public string? PassageText { get; set; }     // Reading only

        public List<QuestionGroup> Groups { get; set; } = new();

    }
}
