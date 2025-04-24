using System.ComponentModel.DataAnnotations;

namespace BOJ0043_Web.Models
{
    public class Workspace
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [StringLength(50, ErrorMessage = "Název může mít maximálně 50 znaků")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Popis je povinný")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "Cena za hodinu musí být v rozmezí 0 až 10000")]
        public decimal PricePerHour { get; set; }

        // Cizí klíč na coworkingový prostor
        public int CoworkingSpaceId { get; set; }
        
        // Navigační vlastnost pro coworkingový prostor
        public virtual CoworkingSpace? CoworkingSpace { get; set; }

        // Aktuální stav pracovního místa
        [Required(ErrorMessage = "Stav pracovního místa je povinný")]
        public WorkspaceStatus CurrentStatus { get; set; } = WorkspaceStatus.Available;

        // Navigační vlastnost pro historii stavů
        public virtual ICollection<WorkspaceStatusHistory> StatusHistory { get; set; } = new List<WorkspaceStatusHistory>();

        // Navigační vlastnost pro rezervace
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    /// <summary>
    /// Výčet možných stavů pracovního místa
    /// </summary>
    public enum WorkspaceStatus
    {
        Available,    // Dostupné
        Occupied,     // Obsazené
        Maintenance   // V údržbě
    }
}
