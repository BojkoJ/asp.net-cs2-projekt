using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BOJ0043_Web.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(
            IReservationRepository reservationRepository,
            IWorkspaceRepository workspaceRepository,
            ILogger<ReservationController> logger)
        {
            _reservationRepository = reservationRepository;
            _workspaceRepository = workspaceRepository;
            _logger = logger;
        }        // GET: Reservation
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return View(reservations);
        }        // GET: Reservation/Details/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationRepository.GetWithWorkspaceAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }        // GET: Reservation/Create
        // Může obsahovat parametr workspaceId pro předvyplnění formuláře
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create(int? workspaceId)
        {
            ViewBag.Workspaces = new SelectList(
                await _workspaceRepository.FindAsync(w => w.CurrentStatus == WorkspaceStatus.Available),
                "Id",
                "Name"
            );

            var model = new Reservation
            {
                StartTime = DateTime.Now.AddHours(1).RoundToNearestHalfHour(),
                EndTime = DateTime.Now.AddHours(2).RoundToNearestHalfHour()
            };

            if (workspaceId.HasValue)
            {
                var workspace = await _workspaceRepository.GetByIdAsync(workspaceId.Value);
                if (workspace != null && workspace.CurrentStatus == WorkspaceStatus.Available)
                {
                    model.WorkspaceId = workspace.Id;
                    model.Workspace = workspace;
                }
            }

            return View(model);
        }        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            // Kontrola dostupnosti pracovního místa
            if (!await _reservationRepository.IsWorkspaceAvailableAsync(
                reservation.WorkspaceId, reservation.StartTime, reservation.EndTime))
            {
                ModelState.AddModelError("", "Vybrané pracovní místo není v požadovaném čase dostupné.");
            }

            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError("EndTime", "Čas konce musí být později než čas začátku.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _reservationRepository.CreateReservationAsync(reservation);
                    await _reservationRepository.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = reservation.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při vytváření rezervace");
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Workspaces = new SelectList(
                await _workspaceRepository.FindAsync(w => w.CurrentStatus == WorkspaceStatus.Available),
                "Id",
                "Name",
                reservation.WorkspaceId
            );
            return View(reservation);
        }        // GET: Reservation/Complete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Complete(int id)
        {
            var reservation = await _reservationRepository.GetWithWorkspaceAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.IsCompleted)
            {
                TempData["Error"] = "Tato rezervace je již ukončena.";
                return RedirectToAction("Details", new { id });
            }

            return View(reservation);
        }        // POST: Reservation/Complete/5
        [HttpPost, ActionName("Complete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CompleteConfirmed(int id)
        {
            try
            {
                await _reservationRepository.CompleteReservationAsync(id);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při ukončování rezervace");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }        // GET: Reservation/Statistics
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Statistics()
        {
            // Výchozí období - poslední měsíc
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-1);

            var statistics = await _reservationRepository.GetReservationStatisticsAsync(startDate, endDate);
            
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            
            return View(statistics);
        }        // POST: Reservation/Statistics
        [HttpPost]
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Statistics(DateTime startDate, DateTime endDate)
        {
            var statistics = await _reservationRepository.GetReservationStatisticsAsync(startDate, endDate);
            
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            
            return View(statistics);
        }
    }

    // Pomocná metoda pro zaokrouhlení času na nejbližší půlhodinu
    public static class DateTimeExtensions
    {
        public static DateTime RoundToNearestHalfHour(this DateTime dt)
        {
            int minute = dt.Minute;
            if (minute < 15)
                minute = 0;
            else if (minute < 45)
                minute = 30;
            else
            {
                minute = 0;
                dt = dt.AddHours(1);
            }

            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, minute, 0);
        }
    }
}
