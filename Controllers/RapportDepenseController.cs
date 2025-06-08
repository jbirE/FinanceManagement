//using FinanceManagement.Data.Dtos;
//using FinanceManagement.Data.Models;
//using FinanceManagement.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace FinanceManagement.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RapportDepenseController : ControllerBase
//    {
//        private readonly RapportDepenseService _rapportDepenseService;

//        public RapportDepenseController(RapportDepenseService rapportDepenseService)
//        {
//            _rapportDepenseService = rapportDepenseService;
//        }

//        [HttpGet("GetAllRapportDepenses")]
//        public async Task<ActionResult<IEnumerable<RapportDepenseDTO>>> GetAll()
//        {
//            try
//            {
//                var rapports = await _rapportDepenseService.GetAllRapportDepenses();
//                return Ok(rapports);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de la récupération des rapports de dépense: {ex.Message}");
//            }
//        }

//        [HttpGet("GetById/{id}")]
//        public async Task<ActionResult<RapportDepenseDTO>> GetById(int id)
//        {
//            try
//            {
//                var rapport = await _rapportDepenseService.GetRapportDepenseById(id);
//                return Ok(rapport);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de la récupération du rapport: {ex.Message}");
//            }
//        }

//        [HttpGet("GetByBudgetProjetIdby-budget/{budgetProjetId}")]
//        public async Task<ActionResult<IEnumerable<RapportDepenseDTO>>> GetByBudgetProjetId(int budgetProjetId)
//        {
//            try
//            {
//                var rapports = await _rapportDepenseService.GetRapportDepensesByBudgetProjetId(budgetProjetId);
//                return Ok(rapports);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
//            }
//        }

//        [HttpGet("by-statut/{statut}")]
//        [Authorize(Roles = "Admin,Manager")]
//        public async Task<ActionResult<IEnumerable<RapportDepenseDTO>>> GetByStatut(RapportDepense.StatutRapport statut)
//        {
//            try
//            {
//                var rapports = await _rapportDepenseService.GetRapportDepensesByStatut(statut);
//                return Ok(rapports);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
//            }
//        }

//        [HttpGet("by-user/{utilisateurId}")]
//        public async Task<ActionResult<IEnumerable<RapportDepenseDTO>>> GetByUtilisateurId(string utilisateurId)
//        {
//            try
//            {
//                var rapports = await _rapportDepenseService.GetRapportDepensesByUtilisateurId(utilisateurId);
//                return Ok(rapports);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
//            }
//        }

//        [HttpPost("AddNewRapportDepense")]
//        public async Task<IActionResult> Create([FromBody] RapportDepenseDTO rapportDepenseDTO)
//        {
//            try
//            {
//                await _rapportDepenseService.AddNewRapportDepense(rapportDepenseDTO);
//                return CreatedAtAction(nameof(GetById), new { id = rapportDepenseDTO.IdRpport }, rapportDepenseDTO);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de la création du rapport: {ex.Message}");
//            }
//        }

//        [HttpPut("UpdateRapportDepense/{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] RapportDepenseDTO rapportDepenseDTO)
//        {
//            try
//            {
//                if (id != rapportDepenseDTO.IdRpport)
//                {
//                    return BadRequest("ID de rapport incohérent");
//                }

//                await _rapportDepenseService.UpdateRapportDepense(rapportDepenseDTO);
//                return NoContent();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de la mise à jour du rapport: {ex.Message}");
//            }
//        }

//        [HttpDelete("DeleteRapportDepense/{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            try
//            {
//                await _rapportDepenseService.DeleteRapportDepense(id);
//                return NoContent();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de la suppression du rapport: {ex.Message}");
//            }
//        }

//        [HttpPost("/{id}/AcceptRapportDepense")]
//        [Authorize(Roles = "Admin,Manager")]
//        public async Task<IActionResult> Approve(int id)
//        {
//            try
//            {
//                await _rapportDepenseService.AcceptRapportDepense(id);
//                return NoContent();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors de l'approbation du rapport: {ex.Message}");
//            }
//        }

//        [HttpPost("{id}/RejeterRapportDepense")]
//        [Authorize(Roles = "Admin,Manager")]
//        public async Task<IActionResult> Reject(int id)
//        {
//            try
//            {
//                await _rapportDepenseService.RejeterRapportDepense(id);
//                return NoContent();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur est survenue lors du rejet du rapport: {ex.Message}");
//            }
//        }
//    }
//}