using System.ComponentModel.DataAnnotations;

namespace BOJ0043_Web.Models
{
    /// <summary>
    /// Model reprezentující historii změn stavu pracovního místa
    /// </summary>
    public class WorkspaceStatusHistory
    {
        public int Id { get; set; }
        
        // Cizí klíč na pracovní místo
        public int WorkspaceId { get; set; }
        
        // Navigační vlastnost pro pracovní místo
        public virtual Workspace? Workspace { get; set; }
        
        // Stav pracovního místa
        [Required(ErrorMessage = "Stav pracovního místa je povinný")]
        public WorkspaceStatus Status { get; set; }
        
        // Čas, kdy byl stav změněn
        [Required(ErrorMessage = "Čas změny je povinný")]
        public DateTime ChangedAt { get; set; }
        
        // Volitelný komentář k změně stavu
        public string? Comment { get; set; }
    }
}
