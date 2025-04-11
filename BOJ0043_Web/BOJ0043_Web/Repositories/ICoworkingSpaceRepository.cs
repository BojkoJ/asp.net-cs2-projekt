using BOJ0043_Web.Models;

namespace BOJ0043_Web.Repositories
{
    public interface ICoworkingSpaceRepository : IRepository<CoworkingSpace>
    {
        /// <summary>
        /// Získá coworkingový prostor včetně všech jeho pracovních míst
        /// </summary>
        Task<CoworkingSpace?> GetWithWorkspacesAsync(int id);

        /// <summary>
        /// Získá všechny coworkingové prostory včetně informací o počtu dostupných pracovních míst
        /// </summary>
        Task<IEnumerable<CoworkingSpace>> GetAllWithAvailableWorkspacesCountAsync();

        /// <summary>
        /// Vyhledá coworkingové prostory podle názvu nebo adresy
        /// </summary>
        Task<IEnumerable<CoworkingSpace>> SearchByNameOrAddressAsync(string searchTerm);
    }
}
