using FinanceManagement.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Services;

namespace FinanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetProjetController : ControllerBase
    {
        private readonly BudgetProjetService _budgetService;

        public BudgetProjetController(BudgetProjetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("GetAllBudgetsProjets")]
        public async Task<ActionResult<IEnumerable<BudgetProjetDto>>> GetAllBudgetsProjets()
        {
            var budgets = await _budgetService.GetAllBudgetsProjetsAsync();
            return Ok(budgets);
        }

        [HttpGet("GetBudgetProjet/{id}")]
        public async Task<ActionResult<BudgetProjetDto>> GetBudgetProjet(int id)
        {
            var budget = await _budgetService.GetBudgetProjetByIdAsync(id);
            if (budget == null)
                return NotFound();
            return Ok(budget);
        }

        [HttpGet("GetBudgetsByProjet/{projetId}")]
        public async Task<ActionResult<IEnumerable<BudgetProjetDto>>> GetBudgetsByProjet(int projetId)
        {
            var budgets = await _budgetService.GetBudgetsByProjetAsync(projetId);
            return Ok(budgets);
        }

        [HttpGet("GetCurrentSpendingForProjet/{projetId}/spending")]
        public async Task<ActionResult<double>> GetCurrentSpendingForProjet(int projetId)
        {
            var spending = await _budgetService.GetCurrentSpendingForProjetAsync(projetId);
            return Ok(spending);
        }

        [HttpGet("IsBudgetOverspent/{id}/overspent")]
        public async Task<ActionResult<bool>> IsBudgetOverspent(int id)
        {
            var isOverspent = await _budgetService.IsBudgetOverspentAsync(id);
            return Ok(isOverspent);
        }

        [HttpPost("CreateBudgetProjet")]
        public async Task<ActionResult<BudgetProjetDto>> CreateBudgetProjet([FromBody] BudgetProjetDto budgetProjetDto)
        {
            try
            {
                var createdBudget = await _budgetService.CreateBudgetProjetAsync(budgetProjetDto);
                return CreatedAtAction(nameof(GetBudgetProjet), new { id = createdBudget.IdBudgetProjet }, createdBudget);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateBudgetProjet/{id}")]
        public async Task<IActionResult> UpdateBudgetProjet(int id, [FromBody] BudgetProjetDto budgetProjetDto)
        {
            try
            {
                if (id != budgetProjetDto.IdBudgetProjet)
                    return BadRequest("ID mismatch");

                await _budgetService.UpdateBudgetProjetAsync(budgetProjetDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteBudgetProjet/{id}")]
        public async Task<IActionResult> DeleteBudgetProjet(int id)
        {
            try
            {
                await _budgetService.DeleteBudgetProjetAsync(id);
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