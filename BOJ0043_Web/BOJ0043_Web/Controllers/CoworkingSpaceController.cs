using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BOJ0043_Web.Controllers
{
    public class CoworkingSpaceController : Controller
    {
        private readonly ICoworkingSpaceRepository _coworkingSpaceRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ILogger<CoworkingSpaceController> _logger;

        public CoworkingSpaceController(
            ICoworkingSpaceRepository coworkingSpaceRepository,
            IWorkspaceRepository workspaceRepository,
            ILogger<CoworkingSpaceController> logger)
        {
            _coworkingSpaceRepository = coworkingSpaceRepository;
            _workspaceRepository = workspaceRepository;
            _logger = logger;
        }        // GET: CoworkingSpace
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Index()
        {
            var spaces = await _coworkingSpaceRepository.GetAllAsync();
            return View(spaces);
        }        // GET: CoworkingSpace/Details/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> Details(int id)
        {
            var space = await _coworkingSpaceRepository.GetWithWorkspacesAsync(id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        // GET: CoworkingSpace/Create
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }        // POST: CoworkingSpace/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Create(CoworkingSpace coworkingSpace)
        {
            if (ModelState.IsValid)
            {
                await _coworkingSpaceRepository.AddAsync(coworkingSpace);
                await _coworkingSpaceRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coworkingSpace);
        }        // GET: CoworkingSpace/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int id)
        {
            var space = await _coworkingSpaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return View(space);
        }        // POST: CoworkingSpace/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int id, CoworkingSpace coworkingSpace)
        {
            if (id != coworkingSpace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _coworkingSpaceRepository.UpdateAsync(coworkingSpace);
                await _coworkingSpaceRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coworkingSpace);
        }        // GET: CoworkingSpace/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int id)
        {
            var space = await _coworkingSpaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return View(space);
        }        // POST: CoworkingSpace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var space = await _coworkingSpaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }

            await _coworkingSpaceRepository.DeleteAsync(space);
            await _coworkingSpaceRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CoworkingSpace/Map
        public async Task<IActionResult> Map()
        {
            var spaces = await _coworkingSpaceRepository.GetAllWithAvailableWorkspacesCountAsync();
            return View(spaces);
        }
    }
}
