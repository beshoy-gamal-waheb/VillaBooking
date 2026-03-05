using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VillaBooking.DTO.Responses;
using VillaBooking.DTO.Villa;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VillaController(IVillaService _villaService, IMapper _mapper) : Controller
    {
        #region Display Villas

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> villaDTOs = new();
            try
            {
                var response = await _villaService.GetAllAsync<APIResponse<List<VillaDTO>>>();
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
        #endregion

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
                var response = await _villaService.CreateAsync<APIResponse<VillaDTO>>(upsertDTO);
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

        #region Delete Villa
        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid villa ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var response = await _villaService.GetAsync<APIResponse<VillaDTO>>(id);
                if (response != null && response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["error"] = response?.Message ?? "Villa not found.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while retrieving villa data: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid villa ID.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var response = await _villaService.DeleteAsync<APIResponse<object>>(id);
                if (response != null && response.Success)
                {
                    TempData["success"] = "Villa deleted successfully!";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Failed to delete the villa.";
                }   
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while deleting the villa: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Edit Villa

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid villa ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var response = await _villaService.GetAsync<APIResponse<VillaDTO>>(id);
                if (response != null && response.Success && response.Data != null)
                {
                    return View(_mapper.Map<VillaUpsertDTO>(response.Data));
                }

                TempData["error"] = response?.Message ?? "Villa not found.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while retrieving villa data: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VillaUpsertDTO upsertDTO)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid villa ID";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                return View(upsertDTO);
            }

            try
            {
                var response = await _villaService.UpdateAsync<APIResponse<VillaDTO>>(id, upsertDTO);
                if (response != null && response.Success && response.Data != null)
                {
                    TempData["success"] = "Villa updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorMessage =
                        response?.Errors is IEnumerable<string> errors && errors.Any()
                        ? string.Join(", ", errors)
                        : response?.Message ?? "An unknown error occurred while updating the villa.";
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while updating the villa: {ex.Message}";
                return View(upsertDTO);
            }
            return View(upsertDTO);
        }
        #endregion
    }
}
