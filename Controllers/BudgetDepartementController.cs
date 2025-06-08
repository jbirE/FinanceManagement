using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Add appropriate authorization
    public class BudgetDepartementController : ControllerBase
    {
        private readonly BudgetDepartementService _budgetService;

        public BudgetDepartementController(BudgetDepartementService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("GetAllBudgetsDepartements")]
        public async Task<ActionResult<IEnumerable<BudgetDepartementDto>>> GetAllBudgetsDepartements()
        {
            var budgets = await _budgetService.GetAllBudgetsDepartementsAsync();
            return Ok(budgets);
        }

        [HttpGet("GetBudgetDepartement/{id}")]
        public async Task<ActionResult<BudgetDepartementDto>> GetBudgetDepartement(int id)
        {
            var budget = await _budgetService.GetBudgetDepartementByIdAsync(id);

            if (budget == null)
                return NotFound();

            return Ok(budget);
        }

        [HttpGet("GetCurrentBudgetForDepartement/{departementId}/year/{year}")]
        public async Task<ActionResult<BudgetDepartementDto>> GetCurrentBudgetForDepartement(int departementId, int year)
        {
            var budget = await _budgetService.GetCurrentBudgetForDepartementAsync(departementId, year);

            if (budget == null)
                return NotFound();

            return Ok(budget);
        }


        [HttpPost("CreateBudgetDepartement")]
        public async Task<ActionResult<BudgetDepartementDto>> CreateBudgetDepartement(BudgetDepartementDto budgetDepartement)
        {
            try
            {
                var createdBudget = await _budgetService.CreateBudgetDepartementAsync(budgetDepartement);
                return CreatedAtAction(nameof(GetBudgetDepartement), new { id = createdBudget.IdBudgetDepartement }, createdBudget);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpPut("UpdateBudgetDepartement/{id}")]
        public async Task<IActionResult> UpdateBudgetDepartement(int id, BudgetDepartementDto budgetDepartement)
        {
            if (id != budgetDepartement.IdBudgetDepartement)
                return BadRequest("ID mismatch");

            budgetDepartement.IdBudgetDepartement = id;
            try
            {
                await _budgetService.UpdateBudgetDepartementAsync(budgetDepartement);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteBudgetDepartement/{id}")]
        public async Task<IActionResult> DeleteBudgetDepartement(int id)
        {
            try
            {
                await _budgetService.DeleteBudgetDepartementAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
