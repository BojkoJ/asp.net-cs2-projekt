using Microsoft.AspNetCore.Identity;

namespace BOJ0043_Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Můžeme přidat další vlastnosti specifické pro naši aplikaci
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
