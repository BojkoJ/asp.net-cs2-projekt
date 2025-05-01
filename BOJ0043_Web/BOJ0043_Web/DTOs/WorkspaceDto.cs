using BOJ0043_Web.Models;

namespace BOJ0043_Web.DTOs
{
    public class WorkspaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerHour { get; set; }
        public int CoworkingSpaceId { get; set; }
        public WorkspaceStatus CurrentStatus { get; set; }
    }
}
