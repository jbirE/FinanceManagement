using FinanceManagement.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Services;

namespace FinanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetController : ControllerBase
    {
        private readonly ProjetService _projetService;

        public ProjetController(ProjetService projetService)
        {
            _projetService = projetService;
        }

        [HttpGet("GetAllProjets")]
        public async Task<ActionResult<IEnumerable<ProjetDto>>> GetAllProjets()
        {
            var projets = await _projetService.GetAllProjetsAsync();
            return Ok(projets);
        }

        [HttpGet("GetProjet/{id}")]
        public async Task<ActionResult<ProjetDto>> GetProjet(int id)
        {
            var projet = await _projetService.GetProjetByIdAsync(id);
            if (projet == null)
                return NotFound();
            return Ok(projet);
        }

        [HttpGet("GetProjetsByDepartement/{departementId}")]
        public async Task<ActionResult<IEnumerable<ProjetDto>>> GetProjetsByDepartement(int departementId)
        {
            var projets = await _projetService.GetProjetsByDepartementAsync(departementId);
            return Ok(projets);
        }

        [HttpPost("CreateProjet")]
        public async Task<ActionResult<ProjetDto>> CreateProjet([FromBody] ProjetDto projetDto)
        {
            try
            {
                var createdProjet = await _projetService.CreateProjetAsync(projetDto);
                return CreatedAtAction(nameof(GetProjet), new { id = createdProjet.IdProjet }, createdProjet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateProjet/{id}")]
        public async Task<IActionResult> UpdateProjet(int id, [FromBody] ProjetDto projetDto)
        {
            try
            {
                if (id != projetDto.IdProjet)
                    return BadRequest("ID mismatch");

                await _projetService.UpdateProjetAsync(projetDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteProjet/{id}")]
        public async Task<IActionResult> DeleteProjet(int id)
        {
            try
            {
                await _projetService.DeleteProjetAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}