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
                    //DepartementNom = user.Departement?.Name,
                    dateEmbauche = user.DateEmbauche,
                    DerniereConnexion = user.DerniereConnexion,
                    Status = user.Status,
                    role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return Ok(userDtos);
        }

        // PUT: api/users/{id}
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto userDto)
        {
            var user = await _userManager.Users.Include(u => u.Departement).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Update allowed properties
            user.Nom = userDto.Nom;
            user.Prenom = userDto.Prenom;
            user.Addresse = userDto.Addresse;
            user.Cin = userDto.Cin;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.IdDepartement = userDto.IdDepartement;
            user.DateEmbauche = userDto.dateEmbauche;
            user.Status = userDto.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Optional: update the user's role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, userDto.Role);

            // Prepare updated user DTO
            var updatedUserDto = new UserDto
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
                dateEmbauche = user.DateEmbauche,
                DerniereConnexion = user.DerniereConnexion,
                Status = user.Status,
                role = userDto.Role
            };

            return Ok(updatedUserDto);
        }



        // DELETE: api/users/{id}

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "User successfully deleted.",
                deletedUser = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Nom,
                    user.Prenom,
                    user.Cin,
                    user.PhoneNumber,
                    user.IdDepartement
                }
            });
        }

    }
}