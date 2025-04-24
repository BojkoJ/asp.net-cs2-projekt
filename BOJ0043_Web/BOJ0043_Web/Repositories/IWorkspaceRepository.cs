using BOJ0043_Web.Models;

namespace BOJ0043_Web.Repositories
{
    public interface IWorkspaceRepository : IRepository<Workspace>
    {
        // Získá pracovní místo včetně coworkingového prostoru
        Task<Workspace?> GetWithCoworkingSpaceAsync(int id);

        // Získá pracovní místo včetně historie stavů
        Task<Workspace?> GetWithStatusHistoryAsync(int id);        // Získá všechna pracovní místa v daném coworkingovém prostoru
        Task<IEnumerable<Workspace>> GetByCoworkingSpaceIdAsync(int coworkingSpaceId);
        
        // Získá všechna pracovní místa včetně jejich coworkingových prostorů
        Task<IEnumerable<Workspace>> GetAllWithCoworkingSpaceAsync();

        // Získá všechna dostupná pracovní místa v daném coworkingovém prostoru
        Task<IEnumerable<Workspace>> GetAvailableByCoworkingSpaceIdAsync(int coworkingSpaceId);

        // Změní stav pracovního místa a zaznamená změnu do historie
        Task ChangeStatusAsync(int id, WorkspaceStatus newStatus, string? comment = null);

        // Získá pracovní místo včetně rezervací
        
        Task <Workspace?> GetWithReservationsAsync(int id);
    }
}
