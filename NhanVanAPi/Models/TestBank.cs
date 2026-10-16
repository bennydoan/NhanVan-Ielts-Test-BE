namespace NhanVanAPi.Models
{
    public class TestBank
    {
        public int Id { get; set; }                     // primary key, auto-increment
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public TestCategory Category { get; set; }
        public bool IsPublished { get; set; } = false; // for admin if they want to publish the test
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
