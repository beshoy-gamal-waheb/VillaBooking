using Microsoft.AspNetCore.Mvc;
using VillaBooking.DTO.Responses;
using VillaBooking.DTO.Villa;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Controllers
{
    public class VillaController(IVillaService _villaService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> villaDTOs = new();
            try
            {
                var response = await _villaService.GetAllAsync<APIResponse<List<VillaDTO>>>("");
                if (response != null && response.Success && response.Data != null)
                {
                    villaDTOs = response.Data;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while retrieving villa data: {ex.Message}";
            }

            return View(villaDTOs);
        }

        #region Create Villa
        
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VillaUpsertDTO upsertDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(upsertDTO);
            }

            try
            {
                var response = await _villaService.CreateAsync<APIResponse<VillaDTO>>(upsertDTO, "");
                if (response != null && response.Success && response.Data != null)
                {
                    TempData["success"] = "Villa created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorMessage =
                        response?.Errors is IEnumerable<string> errors && errors.Any()
                        ? string.Join(", ", errors)
                        : response?.Message ?? "An unknown error occurred while creating the villa.";

                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while creating the villa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            return View(upsertDTO);
        } 

        #endregion
    }
}
