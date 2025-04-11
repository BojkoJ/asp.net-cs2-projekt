using BOJ0043_Web.Data;
using BOJ0043_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BOJ0043_Web.Repositories
{
    public class CoworkingSpaceRepository : Repository<CoworkingSpace>, ICoworkingSpaceRepository
    {
        public CoworkingSpaceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CoworkingSpace?> GetWithWorkspacesAsync(int id)
        {
            return await _context.CoworkingSpaces
                .Include(c => c.Workspaces)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CoworkingSpace>> GetAllWithAvailableWorkspacesCountAsync()
        {
            var spaces = await _context.CoworkingSpaces
                .Include(c => c.Workspaces)
                .ToListAsync();

            // Pro každý prostor spočítáme počet dostupných pracovních míst
            foreach (var space in spaces)
            {
                space.Workspaces = space.Workspaces
                    .Where(w => w.CurrentStatus == WorkspaceStatus.Available)
                    .ToList();
            }

            return spaces;
        }

        public async Task<IEnumerable<CoworkingSpace>> SearchByNameOrAddressAsync(string searchTerm)
        {
            return await _context.CoworkingSpaces
                .Where(c => c.Name.Contains(searchTerm) || c.Address.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
