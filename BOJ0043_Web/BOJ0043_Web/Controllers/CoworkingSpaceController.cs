using BOJ0043_Web.Infrastructure;
using BOJ0043_Web.Models;
using BOJ0043_Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace BOJ0043_Web.Controllers
{
    [DisplayName("Coworkingové prostory")]
    [Description("Správa coworkingových prostorů")]
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
        }

        // GET: CoworkingSpace
        [Authorize(Policy = "RequireReadOnlyRole")]
        [DisplayName("Seznam coworkingových prostorů")]
        [Description("Získá seznam všech coworkingových prostorů")]
        public async Task<IActionResult> Index()
        {
            var spaces = await _coworkingSpaceRepository.GetAllAsync();
            return View(spaces);
        }

        // GET: CoworkingSpace/Details/5
        [Authorize(Policy = "RequireReadOnlyRole")]
        [DisplayName("Detail coworkingového prostoru")]
        [Description("Zobrazí detail konkrétního coworkingového prostoru včetně jeho pracovních míst")]
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
        [DisplayName("Vytvoření coworkingového prostoru - formulář")]
        [Description("Zobrazí formulář pro vytvoření nového coworkingového prostoru")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoworkingSpace/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Vytvoření coworkingového prostoru")]
        [Description("Vytvoří nový coworkingový prostor podle zadaných údajů")]
        public async Task<IActionResult> Create(CoworkingSpace coworkingSpace)
        {
            if (ModelState.IsValid)
            {
                await _coworkingSpaceRepository.AddAsync(coworkingSpace);
                await _coworkingSpaceRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coworkingSpace);
        }

        // GET: CoworkingSpace/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Úprava coworkingového prostoru - formulář")]
        [Description("Zobrazí formulář pro úpravu existujícího coworkingového prostoru")]
        public async Task<IActionResult> Edit(int id)
        {
            var space = await _coworkingSpaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return View(space);
        }

        // POST: CoworkingSpace/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Úprava coworkingového prostoru")]
        [Description("Uloží změny v údajích o coworkingovém prostoru")]
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
        }

        // GET: CoworkingSpace/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Smazání coworkingového prostoru - potvrzení")]
        [Description("Zobrazí stránku pro potvrzení smazání coworkingového prostoru")]
        public async Task<IActionResult> Delete(int id)
        {
            var space = await _coworkingSpaceRepository.GetByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return View(space);
        }

        // POST: CoworkingSpace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        [DisplayName("Smazání coworkingového prostoru")]
        [Description("Provede smazání coworkingového prostoru z databáze")]
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

        #region API metody pro WPF aplikaci

        // GET: CoworkingSpace/GetAll
        [AllowAnonymous] // Povoluje volání bez autentizace
        [HttpGet]
        [ApiSchema(typeof(void), typeof(IEnumerable<BOJ0043_Web.DTOs.CoworkingSpaceDto>))]
        public async Task<JsonResult> GetAll()
        {
            try
            {
                var spaces = await _coworkingSpaceRepository.GetAllAsync();
                var dtos = spaces.Select(s => new BOJ0043_Web.DTOs.CoworkingSpaceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Address = s.Address,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Website = s.Website,
                    PhoneNumber = s.PhoneNumber,
                    Email = s.Email
                }).ToList();
                return Json(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání seznamu coworkingových prostorů");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: CoworkingSpace/GetById/5
        [AllowAnonymous]
        [HttpGet]
        [ApiSchema(typeof(void), typeof(BOJ0043_Web.DTOs.CoworkingSpaceDto))]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var space = await _coworkingSpaceRepository.GetByIdAsync(id);
                if (space == null)
                {
                    return Json(new { error = "Coworkingový prostor nebyl nalezen" });
                }
                var dto = new BOJ0043_Web.DTOs.CoworkingSpaceDto
                {
                    Id = space.Id,
                    Name = space.Name,
                    Description = space.Description,
                    Address = space.Address,
                    Latitude = space.Latitude,
                    Longitude = space.Longitude,
                    PhoneNumber = space.PhoneNumber,
                    Email = space.Email,
                    Website = space.Website
                };
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání coworkingového prostoru s ID {id}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: CoworkingSpace/GetWithWorkspaces/5
        [AllowAnonymous]
        [HttpGet]
        [ApiSchema(typeof(void), typeof(BOJ0043_Web.DTOs.CoworkingSpaceWithWorkspacesDto))]
        public async Task<JsonResult> GetWithWorkspaces(int id)
        {
            try
            {
                var space = await _coworkingSpaceRepository.GetWithWorkspacesAsync(id);
                if (space == null)
                {
                    return Json(new { error = "Coworkingový prostor nebyl nalezen" });
                }
                var dto = new BOJ0043_Web.DTOs.CoworkingSpaceWithWorkspacesDto
                {
                    Id = space.Id,
                    Name = space.Name,
                    Description = space.Description,
                    Address = space.Address,
                    Latitude = space.Latitude,
                    Longitude = space.Longitude,
                    PhoneNumber = space.PhoneNumber,
                    Email = space.Email,
                    Website = space.Website,
                    Workspaces = space.Workspaces?.Select(w => new BOJ0043_Web.DTOs.WorkspaceDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Description = w.Description,
                        PricePerHour = w.PricePerHour,
                        CoworkingSpaceId = w.CoworkingSpaceId,
                        CurrentStatus = w.CurrentStatus
                    }).ToList() ?? new List<BOJ0043_Web.DTOs.WorkspaceDto>()
                };
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání coworkingového prostoru s pracovními místy, ID {id}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // GET: CoworkingSpace/GetWithWorkspacesJson/5
        [AllowAnonymous]
        [HttpGet]
        [ApiSchema(typeof(void), typeof(BOJ0043_Web.DTOs.CoworkingSpaceWithWorkspacesDto))]
        public async Task<JsonResult> GetWithWorkspacesJson(int id)
        {
            try
            {
                var space = await _coworkingSpaceRepository.GetWithWorkspacesAsync(id);
                if (space == null)
                {
                    return Json(new { error = "Coworkingový prostor nebyl nalezen" });
                }
                var dto = new BOJ0043_Web.DTOs.CoworkingSpaceWithWorkspacesDto
                {
                    Id = space.Id,
                    Name = space.Name,
                    Description = space.Description,
                    Address = space.Address,
                    Latitude = space.Latitude,
                    Longitude = space.Longitude,
                    PhoneNumber = space.PhoneNumber,
                    Email = space.Email,
                    Website = space.Website,
                    Workspaces = space.Workspaces?.Select(w => new BOJ0043_Web.DTOs.WorkspaceDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Description = w.Description,
                        PricePerHour = w.PricePerHour,
                        CoworkingSpaceId = w.CoworkingSpaceId,
                        CurrentStatus = w.CurrentStatus
                    }).ToList() ?? new List<BOJ0043_Web.DTOs.WorkspaceDto>()
                };
                return Json(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při získávání coworkingového prostoru s pracovními místy, ID {id}");
                return Json(new { error = "Interní chyba serveru" });
            }
        }

        // PUT: CoworkingSpace/Update/5 (JSON API for WPF)
        [AllowAnonymous]
        [HttpPut]
        [ApiSchema(typeof(BOJ0043_Web.DTOs.CoworkingSpaceDto), typeof(BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto))]
        public async Task<JsonResult> Update(int id, [FromBody] BOJ0043_Web.Models.CoworkingSpace coworkingSpace)
        {
            if (id != coworkingSpace.Id)
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = false, Error = "ID nesouhlasí" });

            try
            {
                await _coworkingSpaceRepository.UpdateAsync(coworkingSpace);
                await _coworkingSpaceRepository.SaveChangesAsync();
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = true, Id = coworkingSpace.Id });
            }
            catch (Exception ex)
            {
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = false, Error = ex.Message });
            }
        }

        // DELETE: CoworkingSpace/DeleteApi/5 (JSON API for WPF)
        [AllowAnonymous]
        [HttpDelete]
        [ApiSchema(typeof(void), typeof(BOJ0043_Web.DTOs.CoworkingSpaceDeleteResultDto))]
        public async Task<JsonResult> DeleteApi(int id)
        {
            try
            {
                var space = await _coworkingSpaceRepository.GetByIdAsync(id);
                if (space == null)
                {
                    return Json(new BOJ0043_Web.DTOs.CoworkingSpaceDeleteResultDto { Success = false, Error = "Coworkingový prostor nebyl nalezen" });
                }

                await _coworkingSpaceRepository.DeleteAsync(space);
                await _coworkingSpaceRepository.SaveChangesAsync();

                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceDeleteResultDto { Success = true, Message = "Coworkingový prostor byl úspěšně smazán" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Chyba při mazání coworkingového prostoru s ID {id}");
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceDeleteResultDto { Success = false, Error = "Interní chyba serveru: " + ex.Message });
            }
        }
        
        // POST: CoworkingSpace/CreateApi (JSON API for WPF)
        [AllowAnonymous]
        [HttpPost]
        [ApiSchema(typeof(BOJ0043_Web.DTOs.CoworkingSpaceDto), typeof(BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto))]
        public async Task<JsonResult> CreateApi([FromBody] BOJ0043_Web.Models.CoworkingSpace coworkingSpace)
        {
            coworkingSpace.Id = 0;

            if (string.IsNullOrWhiteSpace(coworkingSpace.Name))
            {
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = false, Error = "Název coworkingového prostoru je povinný." });
            }

            // Validace na straně serveru - telefon a email
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(coworkingSpace, null, null);
            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(coworkingSpace, context, validationResults, true))
            {
                // Return the first error (or all if you want)
                var errorMessages = validationResults.Select(r => r.ErrorMessage).ToArray();
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = false, Error = string.Join(" ", errorMessages) });
            }

            try
            {
                await _coworkingSpaceRepository.AddAsync(coworkingSpace);
                await _coworkingSpaceRepository.SaveChangesAsync();
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = true, Id = coworkingSpace.Id, Message = "Coworkingový prostor byl úspěšně vytvořen." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při vytváření nového coworkingového prostoru");
                return Json(new BOJ0043_Web.DTOs.CoworkingSpaceCreateResultDto { Success = false, Error = "Interní chyba serveru při vytváření coworkingového prostoru: " + ex.Message });
            }
        }


        #endregion
    }
}
