using FinanceManagement.Data.Dtos;
using FinanceTool.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Controllers
{
    

        [ApiController]
        [Route("api/[controller]")]
        public class DepartementsController : ControllerBase
        {
            private readonly IDepartementRepository _departementRepository;

            public DepartementsController(IDepartementRepository departementRepository)
            {
                _departementRepository = departementRepository;
            }

            [HttpGet("getAlldepartement")]
            public async Task<ActionResult<IEnumerable<DepartementDto>>> GetAllDepartements()
            {
                var departements = await _departementRepository.GetAllDepartementsAsync();
                return Ok(departements);
            }

            [HttpPost("ajouterDepartement")]
            public async Task<ActionResult> AddDepartement(DepartementDto departementDto)
            {
                var createdDepartement = await _departementRepository.AddDepartementAsync(departementDto);
                return Ok(createdDepartement);
            }

            [HttpPut("updateDepartement/{id}")]
            public async Task<ActionResult> UpdateDepartement(int id, DepartementDto departementDto)
            {
                var updatedDepartement = await _departementRepository.UpdateDepartementAsync(id, departementDto);
                if (updatedDepartement == null) return NotFound(new { Message = "Departement not found" });

                return Ok(updatedDepartement);
            }

            [HttpDelete("supprimerDepartement/{id}")]
            public async Task<ActionResult> DeleteDepartement(int id)
            {
                var result = await _departementRepository.DeleteDepartementAsync(id);
                if (!result) return NotFound(new { Message = "Departement not found" });

                return Ok(new { Message = "Departement deleted successfully" });
            }

      
        
    }
    }
