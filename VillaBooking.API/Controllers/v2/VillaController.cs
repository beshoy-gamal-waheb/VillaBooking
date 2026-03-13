using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Models;
using VillaBooking.DTO.Responses;
using VillaBooking.DTO.Villa;

namespace VillaBooking.API.Controllers.v2
{
    //[Authorize(Roles = "Admin")]
    [Route("api/v{version:ApiVersion}/villa")]
    [ApiVersion("2.0")]
    [ApiController]
    public class VillaController(ApplicationDbContext _dbContext,
                                 IMapper _mapper) : ControllerBase
    {
        //[AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas(
            [FromQuery] string? name,
            [FromQuery] string? details,
            [FromQuery] int? minOccupancy,
            [FromQuery] int? maxOccupancy,
            [FromQuery] double? minRate,
            [FromQuery] double? maxRate,
            [FromQuery] int? minSqft,
            [FromQuery] int? maxSqft,
            [FromQuery] string? sortBy, [FromQuery] string? sortOrder = "asc",
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if(page < 1) page = 1;
                if(pageSize < 1) pageSize = 10;
                if(pageSize > 100) pageSize = 100;

                var villasQuery = _dbContext.Villas.AsNoTracking().AsQueryable();

                #region Filtering

                if (minRate.HasValue && maxRate.HasValue && minRate > maxRate)
                {
                    return BadRequest(APIResponse<object>.Error(
                        StatusCodes.Status400BadRequest,
                        "minRate cannot be greater than maxRate"));
                }

                if (minOccupancy.HasValue && maxOccupancy.HasValue && minOccupancy > maxOccupancy)
                {
                    return BadRequest(APIResponse<object>.Error(
                        StatusCodes.Status400BadRequest,
                        "minOccupancy cannot be greater than maxOccupancy"));
                }

                if (minSqft.HasValue && maxSqft.HasValue && minSqft > maxSqft)
                {
                    return BadRequest(APIResponse<object>.Error(
                        StatusCodes.Status400BadRequest,
                        "minSqft cannot be greater than maxSqft"));
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var normalizedName = name.Trim().ToLower();
                    villasQuery = villasQuery.Where(v => v.Name.ToLower().Contains(normalizedName));
                }

                if (!string.IsNullOrWhiteSpace(details))
                {
                    var normalizedDetails = details.Trim().ToLower();
                    villasQuery = villasQuery.Where(v => (v.Details ?? string.Empty).ToLower().Contains(normalizedDetails));
                }

                if (minOccupancy.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Occupancy >= minOccupancy.Value);
                }

                if (maxOccupancy.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Occupancy <= maxOccupancy.Value);
                }

                if (minRate.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Rate >= minRate.Value);
                }

                if (maxRate.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Rate <= maxRate.Value);
                }

                if (minSqft.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Sqft >= minSqft.Value);
                }

                if (maxSqft.HasValue)
                {
                    villasQuery = villasQuery.Where(v => v.Sqft <= maxSqft.Value);
                }

                #endregion

                #region Sorting

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    var isDescending = sortOrder?.Trim().ToLower() == "desc";
                    villasQuery = sortBy.Trim().ToLower() switch
                    {
                        "name" => isDescending ? villasQuery.OrderByDescending(v => v.Name) : villasQuery.OrderBy(v => v.Name),
                        "rate" => isDescending ? villasQuery.OrderByDescending(v => v.Rate) : villasQuery.OrderBy(v => v.Rate),
                        "occupancy" => isDescending ? villasQuery.OrderByDescending(v => v.Occupancy) : villasQuery.OrderBy(v => v.Occupancy),
                        "sqft" => isDescending ? villasQuery.OrderByDescending(v => v.Sqft) : villasQuery.OrderBy(v => v.Sqft),
                        _=> villasQuery.OrderBy(v => v.Id)
                    };
                }
                else
                {
                    villasQuery = villasQuery.OrderBy(v => v.Id);
                }

                #endregion

                #region Pagination

                var totalCount = await villasQuery.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                if (page > totalPages && totalPages > 0)
                {
                    page = totalPages;
                }

                var skip = (page - 1) * pageSize;

                villasQuery = villasQuery.Skip(skip).Take(pageSize);

                #endregion

                var villas = await villasQuery.ToListAsync();
                var dtoResponseVillas = _mapper.Map<IEnumerable<VillaDTO>>(villas);

                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"Successfully retrieved {dtoResponseVillas.Count()} villa/s");
                stringBuilder.Append($" (Page {page} of {totalPages}, {totalCount} total records)");


                if (!string.IsNullOrEmpty(sortBy))
                {
                    stringBuilder.Append($" sorted by: {sortBy} {(sortOrder?.ToLower() == "desc" ? "descending" : "ascending")}");
                }

                Response.Headers.Append("X-Pagination-CurrentPage", page.ToString());
                Response.Headers.Append("X-Pagination-PageSize", pageSize.ToString());
                Response.Headers.Append("X-Pagination-TotalCount", totalCount.ToString());
                Response.Headers.Append("X-Pagination-TotalPages", totalPages.ToString());


                var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVillas, stringBuilder.ToString());
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving villas",
                    ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<String>> GetVillaById(int id)
        {
            return $"This is V2 - {id}";
        }
    }
}
