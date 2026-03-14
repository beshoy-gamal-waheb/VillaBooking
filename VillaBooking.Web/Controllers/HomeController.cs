using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VillaBooking.DTO.Responses;
using VillaBooking.DTO.Villa;
using VillaBooking.Web.Models;
using VillaBooking.Web.Services.IServices;

namespace VillaBooking.Web.Controllers
{
    public class HomeController(IVillaService _villaService) : Controller
    {
        public async Task<IActionResult> Index(VillaQueryParameters query)
        {
            var viewModel = new VillaListViewModel { Query = query };

            try
            {
                var response = await _villaService.GetAllAsync<APIResponse<List<VillaDTO>>>(query);

                if (response != null && response.Success && response.Data != null)
                {
                    viewModel.Villas = response.Data;
                    ParsePaginationMetadata(response.Message, viewModel);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while retrieving villa data: {ex.Message}";
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetVillasAjax(VillaQueryParameters query)
        {
            var viewModel = new VillaListViewModel { Query = query };

            try
            {
                var response = await _villaService.GetAllAsync<APIResponse<List<VillaDTO>>>(query);
                if (response != null && response.Success && response.Data != null)
                {
                    viewModel.Villas = response.Data;
                    ParsePaginationMetadata(response.Message, viewModel);
                }
            }
            catch { }

            return PartialView("_HomeVillasGrid", viewModel);
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

        private static void ParsePaginationMetadata(string? message, VillaListViewModel viewModel)
        {
            if (string.IsNullOrEmpty(message)) return;

            var match = Regex.Match(message, @"Page (\d+) of (\d+), (\d+) total records");
            if (match.Success)
            {
                viewModel.CurrentPage = int.Parse(match.Groups[1].Value);
                viewModel.TotalPages = int.Parse(match.Groups[2].Value);
                viewModel.TotalCount = int.Parse(match.Groups[3].Value);
            }
        }
    }
}

