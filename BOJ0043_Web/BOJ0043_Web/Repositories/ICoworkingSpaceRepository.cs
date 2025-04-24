using BOJ0043_Web.Models;

namespace BOJ0043_Web.Repositories
{
    public interface ICoworkingSpaceRepository : IRepository<CoworkingSpace>
    {
        // Získá coworkingový prostor včetně všech jeho pracovních míst
        Task<CoworkingSpace?> GetWithWorkspacesAsync(int id);

        // Získá všechny coworkingové prostory včetně informací o počtu dostupných pracovních míst
        Task<IEnumerable<CoworkingSpace>> GetAllWithAvailableWorkspacesCountAsync();

        // Vyhledá coworkingové prostory podle názvu nebo adresy
        Task<IEnumerable<CoworkingSpace>> SearchByNameOrAddressAsync(string searchTerm);
    }
}
