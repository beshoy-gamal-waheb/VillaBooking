using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Models;
using VillaBooking.API.Models.DTOs;
using VillaBooking.API.Models.Responses;

namespace VillaBooking.API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController(ApplicationDbContext _dbContext,
                                 IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var villas = await _dbContext.Villas.AsNoTracking().ToListAsync();
            var dtoResponseVillas = _mapper.Map<List<VillaDTO>>(villas);
            var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVillas, "Villas retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillaById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));
                }

                var villa = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
                if (villa is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                } 

                var villaDTO = _mapper.Map<VillaDTO>(villa);

                return Ok(APIResponse<VillaDTO>.Ok(villaDTO, "Records retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving villa with ID {id}",
                    ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> CreateVilla(VillaUpsertDTO villaDTO)
        {
            try
            {
                var duplicateVilla = await _dbContext.Villas.AnyAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla)
                {
                    return Conflict(APIResponse<object>.Conflict($"A Villa with the name '{villaDTO.Name}' already exists"));
                }

                Villa villa = _mapper.Map<VillaUpsertDTO, Villa>(villaDTO);

                await _dbContext.Villas.AddAsync(villa);
                await _dbContext.SaveChangesAsync();

                var dtoResponseVilla = _mapper.Map<VillaDTO>(villa);
                var response = APIResponse<VillaDTO>.CreatedAt(dtoResponseVilla, "Villa created successfully");

                return CreatedAtAction(nameof(GetVillaById), new {id = villa.Id}, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                                "An error occurred while creating the villa",
                                                                ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> UpdateVilla(int id, VillaUpsertDTO villaDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));
                }

                var existingvilla = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
                if (existingvilla is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }

                var duplicateVilla = await _dbContext.Villas.AnyAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower()
                            && v.Id != id);

                if (duplicateVilla)
                {
                    return Conflict(APIResponse<object>.Conflict($"A Villa with the name '{villaDTO.Name}' already exists"));
                }

                _mapper.Map(villaDTO, existingvilla);
                existingvilla.UpdatedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                var dtoResponseVilla = _mapper.Map<VillaDTO>(existingvilla);

                return Ok(APIResponse<VillaDTO>.Ok(dtoResponseVilla, "Villa updated successfully"));

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(StatusCodes.Status500InternalServerError,
                                                                "An error occurred while updating the villa",
                                                                ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID must be greater than 0"));
                }

                var existingVilla = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla is null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }

                _dbContext.Villas.Remove(existingVilla);
                await _dbContext.SaveChangesAsync();

                return Ok(APIResponse<object>.NoContent("Villa deleted successfully"));

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
