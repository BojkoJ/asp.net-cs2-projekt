using System.ComponentModel.DataAnnotations;

namespace BOJ0043_Web.Models
{
    /// <summary>
    /// Model reprezentující coworkingový prostor
    /// </summary>
    public class CoworkingSpace
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [StringLength(100, ErrorMessage = "Název může mít maximálně 100 znaků")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa je povinná")]
        [StringLength(200, ErrorMessage = "Adresa může mít maximálně 200 znaků")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Popis je povinný")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "GPS latitude je povinná")]
        [Range(-90, 90, ErrorMessage = "GPS latitude musí být v rozmezí -90 až 90")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "GPS longitude je povinná")]
        [Range(-180, 180, ErrorMessage = "GPS longitude musí být v rozmezí -180 až 180")]
        public double Longitude { get; set; }

        [Phone(ErrorMessage = "Neplatné telefonní číslo")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Neplatná emailová adresa")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Neplatná URL adresa")]
        public string? Website { get; set; }

        // Navigační vlastnost pro pracovní místa v tomto coworkingovém prostoru
        public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    }
}
