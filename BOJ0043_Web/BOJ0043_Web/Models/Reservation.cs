using System.ComponentModel.DataAnnotations;

namespace BOJ0043_Web.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        // Cizí klíč na pracovní místo
        public int WorkspaceId { get; set; }
        
        // Navigační vlastnost pro pracovní místo
        public virtual Workspace? Workspace { get; set; }
        
        [Required(ErrorMessage = "Email zákazníka je povinný")]
        [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Jméno zákazníka může mít maximálně 100 znaků")]
        public string? CustomerName { get; set; }
        
        [Required(ErrorMessage = "Čas začátku je povinný")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "Čas konce je povinný")]
        public DateTime EndTime { get; set; }
        
        // Délka rezervace v hodinách (vypočítáno automaticky)
        public double DurationHours => (EndTime - StartTime).TotalHours;
        
        // Celková cena rezervace (vypočítáno automaticky)
        public decimal TotalPrice { get; set; }
        
        // Indikátor zda je rezervace dokončená
        public bool IsCompleted { get; set; }
        
        // Volitelná poznámka k rezervaci
        public string? Note { get; set; }
        
        // Čas vytvoření rezervace
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
