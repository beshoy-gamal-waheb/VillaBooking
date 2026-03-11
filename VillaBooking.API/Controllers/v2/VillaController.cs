using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaBooking.API.Data.Contexts;
using VillaBooking.DTO.Villa;
using VillaBooking.API.Models;
using VillaBooking.DTO.Responses;
using Asp.Versioning;

namespace VillaBooking.API.Controllers.v2
{
    //[Authorize(Roles = "Admin")]
    [Route("api/v{version:ApiVersion}/villa")]
    [ApiVersion("2.0")]
    [ApiController]
    public class VillaController(ApplicationDbContext _dbContext,
                                 IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<String>> GetVillas()
        {
            return "This is V2";
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<String>> GetVillaById(int id)
        {
            return $"This is V2 - {id}";
        }
    }
}
