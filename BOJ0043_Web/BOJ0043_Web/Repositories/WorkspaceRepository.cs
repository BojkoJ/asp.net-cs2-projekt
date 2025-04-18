using BOJ0043_Web.Data;
using BOJ0043_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BOJ0043_Web.Repositories
{
    public class WorkspaceRepository : Repository<Workspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Workspace?> GetWithCoworkingSpaceAsync(int id)
        {
            return await _context.Workspaces
                .Include(w => w.CoworkingSpace)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Workspace?> GetWithStatusHistoryAsync(int id)
        {
            return await _context.Workspaces
                .Include(w => w.StatusHistory)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Workspace?> GetWithReservationsAsync(int id)
        {
            return await _context.Workspaces
                .Include(w => w.Reservations)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Workspace>> GetByCoworkingSpaceIdAsync(int coworkingSpaceId)
        {
            return await _context.Workspaces
                .Where(w => w.CoworkingSpaceId == coworkingSpaceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workspace>> GetAvailableByCoworkingSpaceIdAsync(int coworkingSpaceId)
        {
            return await _context.Workspaces
                .Where(w => w.CoworkingSpaceId == coworkingSpaceId && w.CurrentStatus == WorkspaceStatus.Available)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workspace>> GetAllWithCoworkingSpaceAsync()
        {
            return await _context.Workspaces
                .Include(w => w.CoworkingSpace)
                .ToListAsync();
        }

        public async Task ChangeStatusAsync(int id, WorkspaceStatus newStatus, string? comment = null)
        {
            var workspace = await GetByIdAsync(id);
            if (workspace == null)
                throw new ArgumentException($"Pracovní místo s ID {id} nebylo nalezeno.");

            // Kontrola, zda lze změnit stav
            if (workspace.CurrentStatus != WorkspaceStatus.Available && workspace.CurrentStatus != WorkspaceStatus.Maintenance && newStatus != WorkspaceStatus.Available && newStatus != WorkspaceStatus.Maintenance)
                throw new InvalidOperationException("Stav pracovního místa lze změnit pouze mezi 'Dostupné' a 'V údržbě'.");

            // Vytvoření záznamu o změně stavu
            var statusHistory = new WorkspaceStatusHistory
            {
                WorkspaceId = id,
                Status = newStatus,
                ChangedAt = DateTime.Now,
                Comment = comment
            };

            // Aktualizace stavu pracovního místa
            workspace.CurrentStatus = newStatus;

            // Uložení změn
            await _context.WorkspaceStatusHistory.AddAsync(statusHistory);
            await UpdateAsync(workspace);
            await SaveChangesAsync();
        }
    }
}
