using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Implementation;
using FinanceManagement.Repositories.Interface;
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
        private readonly EmailService _emailService ;


        public AuthentificationController(
            UserManager<Utilisateur> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            DataContext dbContext ,
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
                dateEmbauche = userDto.dateEmbauche,
                DerniereConnexion = userDto.DerniereConnexion,
                Status = false,
                IdDepartement = userDto.IdDepartement
            };

            var result = await userManager.CreateAsync(user, userDto.Password);

            string mailBody = $"Welcome to our Platform ! We're thrilled to have you on board.\r\n\r\n" +
                     $"Here are your login credentials:\r\n\r\n" +
                     $"Your Email : {userDto.Email}\r\n" +
                     $"Password: {userDto.Password}\r\n\r\n" +
                     $"Please log in using the link below:\r\n\r\n" +
                     $"[ Application.com.tn]";


            if (result.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, userDto.role);

                await _emailService.SendEmailAsync(userDto.Email, mailBody, userDto.Password);


                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to assign Financier role.", Errors = roleResult.Errors });
                }
                return Ok(new { Message = "user created successfully" });
            }
            return BadRequest(new { Message = "Financier creation failed", Errors = result.Errors });
        }
        
        // Admin-only endpoint to register Admin users
        [HttpPost("AjouterAdmin")]
        public async Task<ActionResult<RegisterDto>> AjouterAdmin([FromForm] RegisterDto userDto)
        {
            // Validate password
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

            var user = new Utilisateur
            {
                UserName = userDto.Username,
                Nom = userDto.Nom,
                Prenom = userDto.Prenom,
                Addresse = userDto.Addresse,
                Cin = userDto.Cin,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to assign Admin role.", Errors = roleResult.Errors });
                }
                return Ok(new { Message = "Admin user registered successfully" });
            }
            return BadRequest(new { Message = "User creation failed", Errors = result.Errors });
        }

        // Password validation method
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            // Check for at least one number and one special character
            var hasNumber = new Regex(@"[0-9]+");
            var hasSpecialChar = new Regex(@"[!@#$%^&*(),.?""{}|<>]+");

            return hasNumber.IsMatch(password) && hasSpecialChar.IsMatch(password);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(); // Invalid email
            }

            var result = await userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return Unauthorized(); // Invalid password
            }
            var roles = await userManager.GetRolesAsync(user);
            Console.WriteLine($"Roles for user {user.UserName}: {string.Join(", ", roles)}");

            var token = GenerateToken(user, roles.ToList());

            var roleToReturn = roles.Any() ? roles.First() : "No role assigned";
            return Ok(new { Token = token, Email = user.Email, Role = roleToReturn, Username = user.UserName });
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
                DepartementNom = currentUser.Departement?.Name
            };

            return Ok(userInfoDto);
        }

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

            return Ok(new { Message = "Password has been reset successfully." });
        }
    }
}