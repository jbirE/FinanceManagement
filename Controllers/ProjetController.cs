using FinanceManagement.Data.Models;
using FinanceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjetController : ControllerBase
    {
        //haka shih 
        private readonly ProjetService _projetService;

        public ProjetController(ProjetService projetService)
        {
            _projetService = projetService;
        }

        [HttpGet("GetAllProjets")]
        public async Task<ActionResult<IEnumerable<Projet>>> GetAllProjets()
        {
            var projets = await _projetService.GetAllProjetsAsync();
            return Ok(projets);
        }

        [HttpGet("GetProjet/{id}")]
        public async Task<ActionResult<Projet>> GetProjet(int id)
        {
            var projet = await _projetService.GetProjetByIdAsync(id);

            if (projet == null)
                return NotFound();

            return Ok(projet);
        }

        [HttpGet("GetProjetsByDepartement/{departementId}")]
        public async Task<ActionResult<IEnumerable<Projet>>> GetProjetsByDepartement(int departementId)
        {
            var projets = await _projetService.GetProjetsByDepartementAsync(departementId);
            return Ok(projets);
        }

        [HttpPost("CreateProjet")]
        public async Task<ActionResult<Projet>> CreateProjet(Projet projet)
        {
            try
            {
                var createdProjet = await _projetService.CreateProjetAsync(projet);
                return CreatedAtAction(nameof(GetProjet), new { id = createdProjet.IdProjet }, createdProjet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateProjet/{id}")]
        public async Task<IActionResult> UpdateProjet(int id, Projet projet)
        {
            if (id != projet.IdProjet)
                return BadRequest("ID mismatch");

            try
            {
                await _projetService.UpdateProjetAsync(projet);
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

