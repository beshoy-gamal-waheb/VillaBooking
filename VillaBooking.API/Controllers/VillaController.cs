using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaBooking.API.Data.Contexts;
using VillaBooking.API.Models;
using VillaBooking.API.Models.DTOs;

namespace VillaBooking.API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController(ApplicationDbContext _dbContext,
                                 IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            var villas = await _dbContext.Villas.ToListAsync();
            var villaDTOs = _mapper.Map<List<VillaDTO>>(villas);
            return Ok(villaDTOs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VillaDTO>> GetVillaById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest("Villa ID must be greater than 0");
                }

                var villa = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
                if (villa is null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                } 

                var villaDTO = _mapper.Map<VillaDTO>(villa);

                return Ok(villaDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving villa with ID {id}: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<VillaDTO>> CreateVilla(VillaUpsertDTO villaDTO)
        {
            try
            {
                var duplicateVilla = await _dbContext.Villas.AnyAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla)
                {
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists");
                }

                Villa villa = _mapper.Map<VillaUpsertDTO, Villa>(villaDTO);

                await _dbContext.Villas.AddAsync(villa);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVillaById), new {id = villa.Id}, _mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"An error occurred while creating the villa : {ex.Message}");
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<VillaDTO>> UpdateVilla(int id, VillaUpsertDTO villaDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Villa ID must be greater than 0");
                }

                var existingvilla = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
                if (existingvilla is null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }

                var duplicateVilla = await _dbContext.Villas.AnyAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower()
                            && v.Id != id);

                if (duplicateVilla)
                {
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists");
                }

                _mapper.Map(villaDTO, existingvilla);
                existingvilla.UpdatedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                return Ok(_mapper.Map<VillaDTO>(existingvilla));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while updating the villa : {ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest("Villa ID must be greater than 0");
                }

                var existingVilla = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla is null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }

                _dbContext.Villas.Remove(existingVilla);
                await _dbContext.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while deleting the villa : {ex.Message}");
            }
        } 

    }
}
