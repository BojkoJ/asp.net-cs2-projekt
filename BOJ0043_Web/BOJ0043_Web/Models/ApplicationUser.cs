using Microsoft.AspNetCore.Identity;

namespace BOJ0043_Web.Models
{
    /// <summary>
    /// Třída reprezentující uživatele aplikace
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Můžeme přidat další vlastnosti specifické pro naši aplikaci
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
