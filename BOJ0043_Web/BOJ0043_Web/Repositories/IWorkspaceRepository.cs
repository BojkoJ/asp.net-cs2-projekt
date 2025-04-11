using BOJ0043_Web.Models;

namespace BOJ0043_Web.Repositories
{
    public interface IWorkspaceRepository : IRepository<Workspace>
    {
        /// <summary>
        /// Získá pracovní místo včetně coworkingového prostoru
        /// </summary>
        Task<Workspace?> GetWithCoworkingSpaceAsync(int id);

        /// <summary>
        /// Získá pracovní místo včetně historie stavů
        /// </summary>
        Task<Workspace?> GetWithStatusHistoryAsync(int id);        /// <summary>
        /// Získá všechna pracovní místa v daném coworkingovém prostoru
        /// </summary>
        Task<IEnumerable<Workspace>> GetByCoworkingSpaceIdAsync(int coworkingSpaceId);
        
        /// <summary>
        /// Získá všechna pracovní místa včetně jejich coworkingových prostorů
        /// </summary>
        Task<IEnumerable<Workspace>> GetAllWithCoworkingSpaceAsync();

        /// <summary>
        /// Získá všechna dostupná pracovní místa v daném coworkingovém prostoru
        /// </summary>
        Task<IEnumerable<Workspace>> GetAvailableByCoworkingSpaceIdAsync(int coworkingSpaceId);

        /// <summary>
        /// Změní stav pracovního místa a zaznamená změnu do historie
        /// </summary>
        Task ChangeStatusAsync(int id, WorkspaceStatus newStatus, string? comment = null);
    }
}
