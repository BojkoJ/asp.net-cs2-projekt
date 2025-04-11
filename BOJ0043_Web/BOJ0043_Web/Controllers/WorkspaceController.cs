using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BOJ0043_Web.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var workspaces = await _workspaceRepository.GetAllWithCoworkingSpaceAsync();
            return View(workspaces);
        }

        // GET: Workspace/Details/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Details(int id)
        {
            var workspace = await _workspaceRepository.GetWithCoworkingSpaceAsync(id);
            if (workspace == null)
            {
                return NotFound();
            }

            return View(workspace);
        }        
        
        // GET: Workspace/Create
        [Authorize(Policy = "RequireAdminRole")]
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
        }          // POST: Workspace/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
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
    }
}
