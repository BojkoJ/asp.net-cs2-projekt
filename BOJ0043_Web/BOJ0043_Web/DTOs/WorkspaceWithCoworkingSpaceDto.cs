using BOJ0043_Web.Models;

namespace BOJ0043_Web.DTOs
{
    public class WorkspaceWithCoworkingSpaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerHour { get; set; }
        public int CoworkingSpaceId { get; set; }
        public WorkspaceStatus CurrentStatus { get; set; }
        public CoworkingSpaceDto CoworkingSpace { get; set; }
    }

    public class CoworkingSpaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
