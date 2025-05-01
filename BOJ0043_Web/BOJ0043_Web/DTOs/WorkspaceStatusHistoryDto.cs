namespace BOJ0043_Web.DTOs
{
    public class WorkspaceStatusHistoryDto
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string Comment { get; set; }
    }
}
