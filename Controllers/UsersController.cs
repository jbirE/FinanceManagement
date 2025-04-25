using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly DataContext _context;

        public UsersController(
            UserManager<Utilisateur> userManager,
            DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/users
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Departement)
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Nom = user.Nom,
                    Prenom = user.Prenom,
                    Addresse = user.Addresse,
                    Cin = user.Cin,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IdDepartement = user.IdDepartement,
                    DepartementNom = user.Departement?.Name,
                    dateEmbauche = user.dateEmbauche,
                    DerniereConnexion = user.DerniereConnexion,
                    Status = user.Status,
                    role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return Ok(userDtos);
        }

        // PUT: api/users/{id}
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Update properties
            user.UserName = userDto.Username;
            user.Email = userDto.Email;
            user.Nom = userDto.Nom;
            user.Prenom = userDto.Prenom;
            user.IdDepartement = userDto.IdDepartement;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        // DELETE: api/users/{id}

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}