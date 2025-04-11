using BOJ0043_Web.Models;

namespace BOJ0043_Web.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        /// <summary>
        /// Získá rezervaci včetně informací o pracovním místě
        /// </summary>
        Task<Reservation?> GetWithWorkspaceAsync(int id);

        /// <summary>
        /// Získá všechny rezervace pro dané pracovní místo
        /// </summary>
        Task<IEnumerable<Reservation>> GetByWorkspaceIdAsync(int workspaceId);

        /// <summary>
        /// Získá všechny aktivní (neukončené) rezervace pro dané pracovní místo
        /// </summary>
        Task<IEnumerable<Reservation>> GetActiveByWorkspaceIdAsync(int workspaceId);

        /// <summary>
        /// Zkontroluje dostupnost pracovního místa v daném časovém období
        /// </summary>
        Task<bool> IsWorkspaceAvailableAsync(int workspaceId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Vytvoří novou rezervaci a změní stav pracovního místa na Obsazené
        /// </summary>
        Task CreateReservationAsync(Reservation reservation);

        /// <summary>
        /// Dokončí rezervaci a změní stav pracovního místa na Dostupné
        /// </summary>
        Task CompleteReservationAsync(int id);

        /// <summary>
        /// Získá statistiku ukončených rezervací pro jednotlivé coworkingové prostory za určité období
        /// </summary>
        Task<Dictionary<int, int>> GetReservationStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}
