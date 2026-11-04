namespace NhanVanAPi.DTOs
{
    public class GetSubmissonHistoryDTO
    {
        public int SubmissionId { get; set; }
        public decimal Band { get; set; }
        public int TimeTakenSeconds { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
