using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using FinanceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly DataContext dbContext;
        private readonly EmailService _emailService;

        public AuthentificationController(
            UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            DataContext dbContext,
            EmailService emailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            config = configuration;
            this.dbContext = dbContext;
            this._emailService = emailService;
        }

        [HttpGet("Departements")]
        public async Task<ActionResult<IEnumerable<Departement>>> GetDepartements()
        {
            var departements = await dbContext.Departements.ToListAsync();
            // ajouter une fonction GetDepartements dans un service departement
            // injection unetOfWork pour acceder à departementRepository 
            // var depart... = await _departementService.getDepartements()
            return Ok(departements);
        }

        [HttpPost("AjouterUser")]
        public async Task<ActionResult<UserDto>> AjouterFinancier([FromForm] UserDto userDto)
        {
            if (!IsValidPassword(userDto.Password))
            {
                return BadRequest(new { Message = "Password must be at least 8 characters long, contain at least one number, and one special character." });
            }

            if (userManager.Users.Any(u => u.Email == userDto.Email))
            {
                return BadRequest(new { Message = "Email already exists." });
            }
            if (userManager.Users.Any(u => u.UserName == userDto.Username))
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            if (!await dbContext.Departements.AnyAsync(d => d.IdDepartement == userDto.IdDepartement))
            {
                return BadRequest(new { Message = "Invalid department ID." });
            }

            var user = new Utilisateur
            {
                UserName = userDto.Username,
                Nom = userDto.Nom,
                Prenom = userDto.Prenom,
                Addresse = userDto.Addresse,
                Cin = userDto.Cin,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                DateEmbauche = userDto.dateEmbauche,
                DerniereConnexion = userDto.DerniereConnexion,
                Status = false, // User is inactive until password is changed
                IdDepartement = userDto.IdDepartement,
                PasswordChanged = false // Ensure this is set to false
            };

            var result = await userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, userDto.role);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to assign role.", Errors = roleResult.Errors });
                }

                // Generate JWT token
                var roles = await userManager.GetRolesAsync(user);
                var token = GenerateToken(user, roles.ToList());

                // Email sending removed for testing purposes

                return Ok(new
                {
                    Message = "User created successfully. Please log in and change your password to activate your account.",
                    Token = token,
                    Email = user.Email,
                    Role = userDto.role,
                    Username = user.UserName
                });
            }
            return BadRequest(new { Message = "User creation failed", Errors = result.Errors });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var result = await userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            // Check if the user needs to change their password
            if (!user.PasswordChanged)
            {
                var roles = await userManager.GetRolesAsync(user);
                var token = GenerateToken(user, roles.ToList());
                return Ok(new
                {
                    Message = "First login detected. Please change your password to activate your account.",
                    MustChangePassword = true,
                    Token = token,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.Any() ? roles.First() : "No role assigned"
                });
            }

            // If password has been changed, proceed with normal login
            var userRoles = await userManager.GetRolesAsync(user);
            Console.WriteLine($"Roles for user {user.UserName}: {string.Join(", ", userRoles)}");

            var loginToken = GenerateToken(user, userRoles.ToList());

            return Ok(new
            {
                Token = loginToken,
                Email = user.Email,
                Role = userRoles.Any() ? userRoles.First() : "No role assigned",
                Username = user.UserName
            });
        }

        [HttpPost("ChangePassword")]
        // [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest(new { Message = "Old and new passwords are required." });
            }

            if (!IsValidPassword(model.NewPassword))
            {
                return BadRequest(new { Message = "New password must be at least 8 characters long, contain at least one number, and one special character." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Password change failed", Errors = result.Errors });
            }

            // Update PasswordChanged and Status
            user.PasswordChanged = true;
            user.Status = true; // Activate the user
            await userManager.UpdateAsync(user);

            return Ok(new { Message = "Password changed successfully. Your account is now active." });
        }

        private string GenerateToken(Utilisateur user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["JWT:Key"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("user-info")]
        // [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUserInfo()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await userManager.Users
                .Include(u => u.Departement)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser == null)
            {
                return NotFound();
            }

            var userInfoDto = new UserDto
            {
                Username = currentUser.UserName,
                Nom = currentUser.Nom,
                Prenom = currentUser.Prenom,
                Cin = currentUser.Cin,
                Password = currentUser.PasswordHash,
                Addresse = currentUser.Addresse,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                // DepartementNom = currentUser.Departement?.Name
            };

            return Ok(userInfoDto);
        }
        //getallusers

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found with this email." });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{config["App:ClientUrl"]}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            return Ok(new { Message = "Reset token generated successfully", ResetLink = resetLink });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid request." });
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Password reset failed", Errors = result.Errors });
            }

            // Update PasswordChanged and Status if this is the first password reset
            if (!user.PasswordChanged)
            {
                user.PasswordChanged = true;
                user.Status = true; // Activate the user
                await userManager.UpdateAsync(user);
            }

            return Ok(new { Message = "Password has been reset successfully." });
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            var hasNumber = new Regex(@"[0-9]+");
            var hasSpecialChar = new Regex(@"[!@#$%^&*(),.?""{}|<>]+");

            return hasNumber.IsMatch(password) && hasSpecialChar.IsMatch(password);
        }
    }
}