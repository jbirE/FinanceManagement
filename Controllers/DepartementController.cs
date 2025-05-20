using FinanceManagement.Data.Dtos;
using FinanceManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartementsController : ControllerBase
    {
        private readonly DepartementService _departementService;

        public DepartementsController(DepartementService departementService)
        {
            _departementService = departementService;
        }

        // GET: api/Departements
        [HttpGet("GetDepartements")]
        public async Task<ActionResult<IEnumerable<DepartementDTO>>> GetDepartements()
        {
            var departements = await _departementService.GetAllDepartementsAsync();
            return Ok(departements);
        }

        // GET: api/Departements/5
        [HttpGet("GetDepartements/{id}")]
        public async Task<ActionResult<DepartementDTO>> GetDepartement(int id)
        {
            var departement = await _departementService.GetDepartementByIdAsync(id);

            if (departement == null)
            {
                return NotFound();
            }

            return Ok(departement);
        }

        // GET: api/Departements/5/details
        [HttpGet("GetDepartementWithDetails/{id}/details")]
        public async Task<ActionResult<DepartementDTO>> GetDepartementWithDetails(int id)
        {
            var departement = await _departementService.GetDepartementWithDetailsAsync(id);

            if (departement == null)
            {
                return NotFound();
            }

            return Ok(departement);
        }

        // POST: api/Departements
        [HttpPost("CreateDepartement")]
        public async Task<ActionResult<DepartementDTO>> CreateDepartement(CreateDepartementDTO departementDto)
        {
            try
            {
                var createdDepartement = await _departementService.CreateDepartementAsync(departementDto);
                return CreatedAtAction(nameof(GetDepartement), new { id = createdDepartement.IdDepartement }, createdDepartement);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Departements/
        [HttpPut("UpdateDepartement/{id}")]
        public async Task<IActionResult> UpdateDepartement(int id, UpdateDepartementDTO departementDto)
        {
            try
            {
                if (!await _departementService.DepartementExistsAsync(id))
                {
                    return NotFound();
                }

                var updatedDepartement = await _departementService.UpdateDepartementAsync(id, departementDto);
                return Ok(updatedDepartement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Departements/5
        [HttpDelete("DeleteDepartement/{id}")]
        public async Task<IActionResult> DeleteDepartement(int id)
        {
            try
            {
                await _departementService.DeleteDepartementAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: api/Departements/exists/5
        [HttpGet("DepartementExists/{id}")]
        public async Task<ActionResult<bool>> DepartementExists(int id)
        {
            return await _departementService.DepartementExistsAsync(id);
        }

        // GET: api/Departements/name-exists
        [HttpGet("name-exists")]
        public async Task<ActionResult<bool>> DepartementNameExists([FromQuery] string name)
        {
            return await _departementService.DepartementNameExistsAsync(name);
        }
    }
}