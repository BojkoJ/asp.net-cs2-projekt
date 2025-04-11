using BOJ0043_Web.Data;
using BOJ0043_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BOJ0043_Web.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public ReservationRepository(ApplicationDbContext context, IWorkspaceRepository workspaceRepository) 
            : base(context)
        {
            _workspaceRepository = workspaceRepository;
        }

        public async Task<Reservation?> GetWithWorkspaceAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Workspace)
                .ThenInclude(w => w.CoworkingSpace)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetByWorkspaceIdAsync(int workspaceId)
        {
            return await _context.Reservations
                .Where(r => r.WorkspaceId == workspaceId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveByWorkspaceIdAsync(int workspaceId)
        {
            return await _context.Reservations
                .Where(r => r.WorkspaceId == workspaceId && !r.IsCompleted)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsWorkspaceAvailableAsync(int workspaceId, DateTime startTime, DateTime endTime)
        {
            // Zkontrolujeme, zda je pracovní místo ve stavu "Dostupné"
            var workspace = await _context.Workspaces.FindAsync(workspaceId);
            if (workspace == null || workspace.CurrentStatus != WorkspaceStatus.Available)
            {
                return false;
            }

            // Zkontrolujeme, zda neexistuje překrývající se rezervace
            var overlappingReservations = await _context.Reservations
                .Where(r => r.WorkspaceId == workspaceId &&
                           !r.IsCompleted &&
                           (
                               // Začátek nové rezervace spadá do stávající rezervace
                               (startTime >= r.StartTime && startTime < r.EndTime) ||
                               // Konec nové rezervace spadá do stávající rezervace
                               (endTime > r.StartTime && endTime <= r.EndTime) ||
                               // Nová rezervace kompletně překrývá stávající rezervaci
                               (startTime <= r.StartTime && endTime >= r.EndTime)
                           ))
                .AnyAsync();

            return !overlappingReservations;
        }

        public async Task CreateReservationAsync(Reservation reservation)
        {
            // Zkontrolujeme dostupnost pracovního místa
            if (!await IsWorkspaceAvailableAsync(reservation.WorkspaceId, reservation.StartTime, reservation.EndTime))
            {
                throw new InvalidOperationException("Pracovní místo není v požadovaném čase dostupné.");
            }

            // Vypočítáme délku rezervace a celkovou cenu
            var workspace = await _context.Workspaces.FindAsync(reservation.WorkspaceId);
            if (workspace == null)
            {
                throw new ArgumentException("Pracovní místo nebylo nalezeno.");
            }

            double durationHours = (reservation.EndTime - reservation.StartTime).TotalHours;
            reservation.TotalPrice = workspace.PricePerHour * (decimal)durationHours;
            reservation.CreatedAt = DateTime.Now;
            reservation.IsCompleted = false;

            // Přidáme rezervaci do databáze
            await _dbSet.AddAsync(reservation);
            
            // Změníme stav pracovního místa na "Obsazené"
            await _workspaceRepository.ChangeStatusAsync(
                reservation.WorkspaceId, 
                WorkspaceStatus.Occupied, 
                $"Vytvoření rezervace pro {reservation.CustomerEmail}"
            );
        }

        public async Task CompleteReservationAsync(int id)
        {
            var reservation = await GetByIdAsync(id);
            if (reservation == null)
            {
                throw new ArgumentException("Rezervace nebyla nalezena.");
            }

            // Kontrola, zda již není ukončená
            if (reservation.IsCompleted)
            {
                throw new InvalidOperationException("Tato rezervace je již ukončena.");
            }

            // Ukončíme rezervaci
            reservation.IsCompleted = true;
            await UpdateAsync(reservation);

            // Zkontrolujeme, zda má pracovní místo ještě jiné aktivní rezervace
            var hasActiveReservations = await _context.Reservations
                .Where(r => r.WorkspaceId == reservation.WorkspaceId && !r.IsCompleted && r.Id != id)
                .AnyAsync();

            // Pokud nemá jiné aktivní rezervace, změníme stav na "Dostupné"
            if (!hasActiveReservations)
            {
                await _workspaceRepository.ChangeStatusAsync(
                    reservation.WorkspaceId,
                    WorkspaceStatus.Available,
                    $"Ukončení rezervace pro {reservation.CustomerEmail}"
                );
            }

            await SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllWithWorkspaceAndCoworkingSpaceAsync()
        {
            return await _context.Reservations
                .Include(r => r.Workspace)
                .ThenInclude(w => w.CoworkingSpace)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<Dictionary<int, (string Name, int Count)>> GetReservationStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var completedReservations = await _context.Reservations
                .Where(r => r.IsCompleted && r.EndTime >= startDate && r.EndTime <= endDate)
                .Include(r => r.Workspace)
                .ThenInclude(w => w.CoworkingSpace)
                .ToListAsync();

            // Seskupíme rezervace podle coworkingového prostoru a spočítáme jejich počet
            var result = new Dictionary<int, (string Name, int Count)>();
            
            foreach (var group in completedReservations.GroupBy(r => r.Workspace.CoworkingSpaceId))
            {
                // Získáme jméno coworkingového prostoru z první rezervace ve skupině
                var spaceName = group.FirstOrDefault()?.Workspace?.CoworkingSpace?.Name ?? "Neznámý prostor";
                result.Add(group.Key, (spaceName, group.Count()));
            }
            
            return result;
        }

        public async Task<int> AutoCompleteExpiredReservationsAsync()
        {
            // Najdeme všechny neukončené rezervace, jejichž čas konce již uplynul
            var expiredReservations = await _context.Reservations
                .Where(r => !r.IsCompleted && r.EndTime < DateTime.Now)
                .ToListAsync();

            int completedCount = 0;

            foreach (var reservation in expiredReservations)
            {
                try
                {
                    // Ukončíme rezervaci
                    reservation.IsCompleted = true;
                    _context.Reservations.Update(reservation);

                    // Zkontrolujeme, zda má pracovní místo ještě jiné aktivní rezervace
                    var hasActiveReservations = await _context.Reservations
                        .Where(r => r.WorkspaceId == reservation.WorkspaceId && !r.IsCompleted && r.Id != reservation.Id)
                        .AnyAsync();

                    // Pokud nemá jiné aktivní rezervace, změníme stav na "Dostupné"
                    if (!hasActiveReservations)
                    {
                        await _workspaceRepository.ChangeStatusAsync(
                            reservation.WorkspaceId,
                            WorkspaceStatus.Available,
                            $"Automatické ukončení rezervace pro {reservation.CustomerEmail}"
                        );
                    }

                    completedCount++;
                }
                catch (Exception)
                {
                    // Pokračujeme s dalšími rezervacemi i v případě chyby
                    continue;
                }
            }

            await _context.SaveChangesAsync();
            return completedCount;
        }
    }
}
