namespace BOJ0043_Web.DTOs
{
    public class CoworkingSpaceCreateResultDto
    {
        public bool Success { get; set; }
        public int? Id { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
