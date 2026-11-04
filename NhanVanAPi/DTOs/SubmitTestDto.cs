namespace NhanVanAPi.DTOs
{
    public class SubmitTestDto
    {
        public int TestId { get; set; }
        public Dictionary<int, string?> Answers { get; set; } = new(); // { 1: "train", 2: null, ... }, key value pair
        public int TimeTakenSeconds { get; set; }

    }
}
