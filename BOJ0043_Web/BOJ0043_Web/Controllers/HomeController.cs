using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BOJ0043_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICoworkingSpaceRepository _coworkingSpaceRepository;
        private readonly IWorkspaceRepository _workspaceRepository;

        public HomeController(
            ILogger<HomeController> logger,
            ICoworkingSpaceRepository coworkingSpaceRepository,
            IWorkspaceRepository workspaceRepository)
        {
            _logger = logger;
            _coworkingSpaceRepository = coworkingSpaceRepository;
            _workspaceRepository = workspaceRepository;
        }        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Policy = "RequireReadOnlyRole")]
        public async Task<IActionResult> GetCoworkingSpacesForMap()
        {
            try
            {
                var spaces = await _coworkingSpaceRepository.GetAllAsync();
                var result = new List<object>(); // Změna z List<dynamic> na List<object>

                foreach (var space in spaces)
                {
                    var workspaces = await _workspaceRepository.GetByCoworkingSpaceIdAsync(space.Id);
                    int availableCount = 0;
                    int occupiedCount = 0;
                    int maintenanceCount = 0;
                    
                    foreach (var workspace in workspaces)
                    {
                        if (workspace.CurrentStatus == WorkspaceStatus.Available)
                            availableCount++;
                        else if (workspace.CurrentStatus == WorkspaceStatus.Occupied)
                            occupiedCount++;
                        else if (workspace.CurrentStatus == WorkspaceStatus.Maintenance)
                            maintenanceCount++;
                    }
                    
                    //var totalCount = workspaces.Count;
                    int totalCount = 0;

                    foreach (var workspace in workspaces)
                    {
                        totalCount++;
                    }
                    
                    var spaceData = new
                    {
                        id = space.Id,
                        name = space.Name,
                        latitude = space.Latitude,
                        longitude = space.Longitude,
                        address = space.Address,
                        availableWorkspaces = availableCount,
                        occupiedWorkspaces = occupiedCount,
                        maintenanceWorkspaces = maintenanceCount,
                        totalWorkspaces = totalCount
                    };
                    
                    result.Add(spaceData);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání dat pro mapu");
                return Json(new { error = "Nepodařilo se načíst data pro mapu" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
