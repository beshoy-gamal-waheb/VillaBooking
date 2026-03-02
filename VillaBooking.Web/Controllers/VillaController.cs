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
                if(response != null && response.Success && response.Data != null)
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
    }
}
