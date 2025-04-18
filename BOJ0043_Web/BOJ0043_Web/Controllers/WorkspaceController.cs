using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace BOJ0043_Web.Controllers
{
    [DisplayName("Pracovní místa")]
    [Description("Správa pracovních míst")]
    public class WorkspaceController : Controller
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICoworkingSpaceRepository _coworkingSpaceRepository;
        private readonly ILogger<WorkspaceController> _logger;

        public WorkspaceController(
            IWorkspaceRepository workspaceRepository,
            ICoworkingSpaceRepository coworkingSpaceRepository,
            ILogger<WorkspaceController> logger)
        {
            _workspaceRepository = workspaceRepository;
            _coworkingSpaceRepository = coworkingSpaceRepository;
            _logger = logger;
        }

        // GET: Workspace
        [Authorize(Policy = "RequireReadOnlyRole")]
        [DisplayName("Seznam pracovních míst")]
        [Description("Získá seznam všech pracovních míst")]
        public async Task<IActionResult> Index()
        {
            var workspaces = await _workspaceRepository.GetAllWithCoworkingSpaceAsync();
            return View(workspaces);
        }

        // GET: Workspace/Details/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        [DisplayName("Detail pracovního místa")]
        [Description("Zobrazí detail konkrétního pracovního místa včetně jeho historie stavu")]
        public async Task<IActionResult> Details(int id)
        {
            var workspace = await _workspaceRepository.GetWithReservationsAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            return View(workspace);
        }

        // GET: Workspace/Create
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Vytvoření pracovního místa - formulář")]
        [Description("Zobrazí formulář pro vytvoření nového pracovního místa")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CoworkingSpaces = new SelectList(
                await _coworkingSpaceRepository.GetAllAsync(),
                "Id",
                "Name"
            );
            return View();
        }

        // POST: Workspace/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Vytvoření pracovního místa")]
        [Description("Vytvoří nové pracovní místo podle zadaných údajů")]
        public async Task<IActionResult> Create(Workspace workspace)
        {
            if (ModelState.IsValid)
            {
                // Nastavení výchozího stavu na "Dostupné"
                workspace.CurrentStatus = WorkspaceStatus.Available;

                await _workspaceRepository.AddAsync(workspace);
                await _workspaceRepository.SaveChangesAsync();

                // Vytvoření prvního záznamu v historii stavů
                await _workspaceRepository.ChangeStatusAsync(workspace.Id, WorkspaceStatus.Available, "Vytvoření pracovního místa");

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CoworkingSpaces = new SelectList(
                await _coworkingSpaceRepository.GetAllAsync(),
                "Id",
                "Name"
            );
            return View(workspace);
        }

        // GET: Workspace/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Úprava pracovního místa - formulář")]
        [Description("Zobrazí formulář pro úpravu existujícího pracovního místa")]
        public async Task<IActionResult> Edit(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            ViewBag.CoworkingSpaces = new SelectList(
                await _coworkingSpaceRepository.GetAllAsync(),
                "Id",
                "Name",
                workspace.CoworkingSpaceId
            );
            return View(workspace);
        }

        // POST: Workspace/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Úprava pracovního místa")]
        [Description("Uloží změny v údajích o pracovním místě")]
        public async Task<IActionResult> Edit(int id, Workspace workspace)
        {
            if (id != workspace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Získáme původní stav z databáze
                    var originalWorkspace = await _workspaceRepository.GetByIdAsync(id);
                    if (originalWorkspace == null)
                    {
                        return NotFound();
                    }

                    // Aktualizujeme vlastnosti původního objektu
                    originalWorkspace.Name = workspace.Name;
                    originalWorkspace.Description = workspace.Description;
                    originalWorkspace.PricePerHour = workspace.PricePerHour;
                    originalWorkspace.CoworkingSpaceId = workspace.CoworkingSpaceId;

                    // Pokud se změnil stav, zaznamenáme změnu do historie
                    if (originalWorkspace.CurrentStatus != workspace.CurrentStatus)
                    {
                        await _workspaceRepository.ChangeStatusAsync(id, workspace.CurrentStatus, "Úprava pracovního místa");
                    }
                    else
                    {
                        // Použijeme původní objekt pro update - nepoužíváme UpdateAsync, který by způsobil attach entity
                        // Místo toho rovnou uložíme změny, protože originalWorkspace je již sledován kontextem
                        await _workspaceRepository.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Chyba při aktualizaci pracovního místa");
                    ModelState.AddModelError("", "Došlo k chybě při ukládání změn: " + ex.Message);
                }
            }

            ViewBag.CoworkingSpaces = new SelectList(
                await _coworkingSpaceRepository.GetAllAsync(),
                "Id",
                "Name",
                workspace.CoworkingSpaceId
            );
            return View(workspace);
        }

        // GET: Workspace/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Smazání pracovního místa - potvrzení")]
        [Description("Zobrazí stránku pro potvrzení smazání pracovního místa")]
        public async Task<IActionResult> Delete(int id)
        {
            var workspace = await _workspaceRepository.GetWithCoworkingSpaceAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }
            return View(workspace);
        }

        // POST: Workspace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Smazání pracovního místa")]
        [Description("Provede smazání pracovního místa z databáze")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            await _workspaceRepository.DeleteAsync(workspace);
            await _workspaceRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Workspace/StatusHistory/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        [DisplayName("Historie stavů pracovního místa")]
        [Description("Zobrazí historii změn stavů pracovního místa")]
        public async Task<IActionResult> StatusHistory(int id)
        {
            var workspace = await _workspaceRepository.GetWithStatusHistoryAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }
            return View(workspace);
        }

        // GET: Workspace/ChangeStatus/5
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Změna stavu pracovního místa - formulář")]
        [Description("Zobrazí formulář pro změnu stavu pracovního místa")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            ViewBag.CurrentStatus = workspace.CurrentStatus;
            return View(workspace);
        }

        // POST: Workspace/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Změna stavu pracovního místa")]
        [Description("Provede změnu stavu pracovního místa a uloží ji do historie")]
        public async Task<IActionResult> ChangeStatus(int id, WorkspaceStatus newStatus, string comment)
        {
            try
            {
                await _workspaceRepository.ChangeStatusAsync(id, newStatus, comment);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při změně stavu pracovního místa");
                ModelState.AddModelError("", ex.Message);

                var workspace = await _workspaceRepository.GetByIdAsync(id);
                if (workspace == null)
                {
                    return NotFound();
                }

                ViewBag.CurrentStatus = workspace.CurrentStatus;
                return View(workspace);
            }
        }

        #region API metody pro WPF aplikaci

        // GET: Workspace/GetAll
        [AllowAnonymous] // Povoluje volání bez autentizace
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            try
            {
                var workspaces = await _workspaceRepository.GetAllAsync();
                return Json(workspaces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání seznamu pracovních míst");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: Workspace/GetAllWithCoworkingSpace
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetAllWithCoworkingSpace()
        {
            try
            {
                var workspaces = await _workspaceRepository.GetAllWithCoworkingSpaceAsync();
                return Json(workspaces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání seznamu pracovních míst s coworkingovými prostory");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: Workspace/GetById/5
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var workspace = await _workspaceRepository.GetByIdAsync(id);
                if (workspace == null)
                {
                    return Json(new { error = "Pracovní místo nebylo nalezeno" });
                }
                return Json(workspace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání pracovního místa s ID {id}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: Workspace/GetWithCoworkingSpace/5
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetWithCoworkingSpace(int id)
        {
            try
            {
                var workspace = await _workspaceRepository.GetWithCoworkingSpaceAsync(id);
                if (workspace == null)
                {
                    return Json(new { error = "Pracovní místo nebylo nalezeno" });
                }
                return Json(workspace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání pracovního místa s coworkingovým prostorem, ID {id}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: Workspace/GetByCoworkingSpaceId/5
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetByCoworkingSpaceId(int coworkingSpaceId)
        {
            try
            {
                var workspaces = await _workspaceRepository.GetByCoworkingSpaceIdAsync(coworkingSpaceId);
                return Json(workspaces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání pracovních míst pro coworkingový prostor s ID {coworkingSpaceId}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: Workspace/GetStatusHistory?workspaceId=5
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetStatusHistory(int workspaceId)
        {
            try
            {
                var workspace = await _workspaceRepository.GetWithStatusHistoryAsync(workspaceId);
                if (workspace == null)
                {
                    return Json(new { error = "Pracovní místo nebylo nalezeno" });
                }
                // Project to avoid cycles: only send primitive properties
                var history = workspace.StatusHistory
                    .Select(h => new
                    {
                        h.Id,
                        h.WorkspaceId,
                        Status = h.Status.ToString(),
                        h.ChangedAt,
                        h.Comment
                    }).ToList();
                return Json(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání historie stavů pro pracovní místo s ID {workspaceId}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // POST: Workspace/ChangeStatusApi
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ChangeStatusApi(int id, string newStatus, string comment)
        {
            try
            {
                // Parse the status string to enum
                if (!Enum.TryParse<WorkspaceStatus>(newStatus, out var statusEnum))
                {
                    return Json(new { success = false, error = "Neplatný stav." });
                }
                await _workspaceRepository.ChangeStatusAsync(id, statusEnum, comment);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při změně stavu pracovního místa (API) pro ID {id}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // PUT: Workspace/Update/{id} (JSON API for WPF)
        [AllowAnonymous]
        [HttpPut]
        public async Task<JsonResult> Update(int id, [FromBody] Workspace workspace)
        {
            if (id != workspace.Id)
                return Json(new { error = "ID nesouhlasí" });

            try
            {
                var originalWorkspace = await _workspaceRepository.GetByIdAsync(id);
                if (originalWorkspace == null)
                    return Json(new { error = "Pracovní místo nebylo nalezeno" });

                // Update properties
                originalWorkspace.Name = workspace.Name;
                originalWorkspace.Description = workspace.Description;
                originalWorkspace.PricePerHour = workspace.PricePerHour;
                originalWorkspace.CoworkingSpaceId = workspace.CoworkingSpaceId;
                originalWorkspace.CurrentStatus = workspace.CurrentStatus;

                await _workspaceRepository.SaveChangesAsync();
                return Json(new { success = true, id = originalWorkspace.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při aktualizaci pracovního místa (API)");
                return Json(new { error = ex.Message });
            }
        }

        // POST: Workspace/CreateApi (JSON API for WPF)
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> CreateApi([FromBody] Workspace workspace)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(workspace.Name))
                return Json(new { error = "Název pracovního místa je povinný." });

            try
            {
                await _workspaceRepository.AddAsync(workspace);
                await _workspaceRepository.SaveChangesAsync();
                return Json(new { success = true, id = workspace.Id, message = "Pracovní místo bylo úspěšně vytvořeno." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při vytváření nového pracovního místa");
                return Json(new { error = "Interní chyba serveru při vytváření pracovního místa: " + ex.Message });
            }
        }

        // DELETE: Workspace/DeleteApi/5 (JSON API for WPF)
        [AllowAnonymous]
        [HttpDelete]
        public async Task<JsonResult> DeleteApi(int id)
        {
            try
            {
                var workspace = await _workspaceRepository.GetByIdAsync(id);
                if (workspace == null)
                {
                    return Json(new { success = false, error = "Pracovní místo nebylo nalezeno." });
                }

                await _workspaceRepository.DeleteAsync(workspace);
                await _workspaceRepository.SaveChangesAsync();
                return Json(new { success = true, message = "Pracovní místo bylo úspěšně smazáno." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při mazání pracovního místa (API) s ID {id}");
                // Consider more specific error handling, e.g., foreign key constraints
                return Json(new { success = false, error = "Interní chyba serveru při mazání pracovního místa: " + ex.Message });
            }
        }


        #endregion
    }
}
