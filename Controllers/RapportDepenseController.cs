//using System.Collections.Generic;
//using System.Threading.Tasks;
//using AutoMapper;
//using FinanceManagement.Data.Models;
//using FinanceManagement.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace FinanceManagement.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RapportDepenseController : ControllerBase
//    {
//        private readonly RapportDepenseService _rapportService;
//        private readonly IMapper _mapper;

//        public RapportDepenseController(RapportDepenseService rapportService, IMapper mapper)
//        {
//            _rapportService = rapportService;
//            _mapper = mapper;
//        }

//        [HttpGet("GetAllRapports")]
//        public async Task<ActionResult<IEnumerable<RapportDepense>>> GetAllRapports()
//        {
//            var rapports = await _rapportService.GetAllRapportsAsync();
//            return Ok(rapports);
//        }

//        [HttpGet("GetRapportById/{id}")]
//        public async Task<ActionResult<RapportDepense>> GetRapportById(int id)
//        {
//            var rapport = await _rapportService.GetRapportByIdAsync(id);

//            if (rapport == null)
//            {
//                return NotFound();
//            }

//            return Ok(rapport);
//        }

//        [HttpGet("GetRapportsByBudgetProjet/{budgetProjetId}")]
//        public async Task<ActionResult<IEnumerable<RapportDepense>>> GetRapportsByBudgetProjet(int budgetProjetId)
//        {
//            var rapports = await _rapportService.GetRapportsByBudgetProjetIdAsync(budgetProjetId);
//            return Ok(rapports);
//        }

//        [HttpGet("GetRapportsByStatut/{statut}")]
//        public async Task<ActionResult<IEnumerable<RapportDepense>>> GetRapportsByStatut(RapportDepense.StatutRapport statut)
//        {
//            var rapports = await _rapportService.GetRapportsByStatutAsync(statut);
//            return Ok(rapports);
//        }

//        [HttpGet("GetRapportsByUtilisateur/{utilisateurId}")]
//        public async Task<ActionResult<IEnumerable<RapportDepense>>> GetRapportsByUtilisateur(string utilisateurId)
//        {
//            var rapports = await _rapportService.GetRapportsByUtilisateurIdAsync(utilisateurId);
//            return Ok(rapports);
//        }

//        [HttpPost("CreateRapport")]
//        public async Task<ActionResult<RapportDepense>> CreateRapport(RapportDepense rapportDepense)
//        {
//            await _rapportService.CreateRapportAsync(rapportDepense);

//            return CreatedAtAction(nameof(GetRapportById), new { id = rapportDepense.IdRpport }, rapportDepense);
//        }

//        [HttpPut("UpdateRapport/{id}")]
//        public async Task<IActionResult> UpdateRapport(int id, RapportDepense rapportDepense)
//        {
//            if (id != rapportDepense.IdRpport)
//            {
//                return BadRequest();
//            }

//            var exists = await _rapportService.RapportExistsAsync(id);
//            if (!exists)
//            {
//                return NotFound();
//            }

//            await _rapportService.UpdateRapportAsync(rapportDepense);

//            return NoContent();
//        }

//        [HttpDelete("DeleteRapport/{id}")]
//        public async Task<IActionResult> DeleteRapport(int id)
//        {
//            var exists = await _rapportService.RapportExistsAsync(id);
//            if (!exists)
//            {
//                return NotFound();
//            }

//            await _rapportService.DeleteRapportAsync(id);

//            return NoContent();
//        }
//    }
//}