using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VillaBooking.DTO.Responses;
using VillaBooking.DTO.Villa;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Controllers
{
    public class HomeController(IVillaService _villaService) : Controller
    {
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
