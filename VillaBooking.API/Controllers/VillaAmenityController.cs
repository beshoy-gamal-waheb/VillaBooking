using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.DTOs.Villa;
using VillaBooking.API.DTOs.VillaAmenity;
using VillaBooking.API.Models;
using VillaBooking.API.Models.Responses;

namespace VillaBooking.API.Controllers
{
    [Route("api/villa-amenities")]
    [ApiController]
    public class VillaAmenityController(ApplicationDbContext _dbContext, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaAmenityDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaAmenityDTO>>>> GetVillaAmenities()
        {
            try
            {
                var villaAmenities = await _dbContext.VillaAmenities.AsNoTracking().Include(v => v.Villa).ToListAsync();
                var villaAmenitiesDTO = _mapper.Map<List<VillaAmenityDTO>>(villaAmenities);
                var response = APIResponse<IEnumerable<VillaAmenityDTO>>.Ok(villaAmenitiesDTO, "Villa Amenities retrieved successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              $"An error occurred while retrieving villa amenities",
                                                              ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> GetVillaAmenityById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenity ID must be greater than 0"));
                }

                var villaAmenity = await _dbContext.VillaAmenities.AsNoTracking().Include(v => v.Villa).FirstOrDefaultAsync(v => v.Id == id);
                if (villaAmenity is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa amenity with ID {id} was not found"));
                }

                var villaAmenityDTO = _mapper.Map<VillaAmenityDTO>(villaAmenity);
                var response = APIResponse<VillaAmenityDTO>.Ok(villaAmenityDTO, "Records retrieved successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              $"An error occurred while retrieving villa amenity with ID {id}",
                                                              ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> CreateVillaAmenity(VillaAmenityCreateDTO amenityCreateDTO)
        {
            try
            {
                if (amenityCreateDTO is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenity data is required"));
                }

                if (amenityCreateDTO.VillaId <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));
                }

                var villaExists = await _dbContext.Villas.AnyAsync(v => v.Id == amenityCreateDTO.VillaId);
                if (!villaExists)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {amenityCreateDTO.VillaId} was not found"));
                }

                var villaAmenity = _mapper.Map<VillaAmenity>(amenityCreateDTO);
                await _dbContext.VillaAmenities.AddAsync(villaAmenity);
                await _dbContext.SaveChangesAsync();

                await _dbContext.Entry(villaAmenity)
                    .Reference(v => v.Villa)
                    .LoadAsync();

                var dtoResponseVillaAmenity = _mapper.Map<VillaAmenityDTO>(villaAmenity);
                var response = APIResponse<VillaAmenityDTO>.CreatedAt(dtoResponseVillaAmenity, "Villa amenity created successfully");
                return CreatedAtAction(nameof(GetVillaAmenityById), new {id = villaAmenity.Id}, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              "An error occurred while creating the villa amenity",
                                                              ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> UpdateVillaAmenity(int id, VillaAmenityUpdateDTO amenityUpdateDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenity ID must be greater than 0"));
                }

                if (amenityUpdateDTO is null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenity data is required"));
                }

                var existingVillaAmenity = await _dbContext.VillaAmenities.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVillaAmenity is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa amenity with ID {id} was not found"));
                }

                _mapper.Map(amenityUpdateDTO, existingVillaAmenity);
                existingVillaAmenity.UpdatedDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                await _dbContext.Entry(existingVillaAmenity)
                    .Reference(v => v.Villa)
                    .LoadAsync();

                var dtoResponseVillaAmenity = _mapper.Map<VillaAmenityDTO>(existingVillaAmenity);
                var response = APIResponse<VillaAmenityDTO>.Ok(dtoResponseVillaAmenity, "Villa amenity updated successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              "An error occurred while updating the villa amenity",
                                                              ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVillaAmenity(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa amenity ID must be greater than 0"));
                }

                var villaAmenity = await _dbContext.VillaAmenities.FirstOrDefaultAsync(v => v.Id == id);
                if (villaAmenity is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa amenity with ID {id} was not found"));
                }

                _dbContext.VillaAmenities.Remove(villaAmenity);
                await _dbContext.SaveChangesAsync();

                var response = APIResponse<object>.NoContent("Villa amenity deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                              "An error occurred while deleting the villa",
                                                              ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
