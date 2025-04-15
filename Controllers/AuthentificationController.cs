using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace FinanceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AuthentificationController(UserManager<Utilisateur> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            config = configuration;
        }
        [HttpPost("AjouterFinancier")]
        public async Task<ActionResult<FinancierDto>> Register([FromForm] FinancierDto userDto)
        {
            if (userManager.Users.Any(u => u.Email == userDto.Email))
            {
                return BadRequest(new { Message = "Email already exist." });
            }
            if (userManager.Users.Any(u => u.UserName == userDto.Username))
            {
                return BadRequest(new { Message = "UserName already exist." });
            }

            var user = new Utilisateur
            {
                UserName = userDto.Username,
                PasswordHash = userDto.Password,
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
                var roleResult = await userManager.AddToRoleAsync(user, "Financier");


                return Ok(new { Message = "User Added Successfully" });
            }
            return BadRequest(new { Message = "User creation failed ", Errors = result.Errors });
        }

            [HttpPost("Login")]

            public async Task<IActionResult>Login([FromBody] LoginModel model)
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
                // Log the roles for debugging
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

            // Include role claims
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
        //ici une api permet de recuperer les cordonnées de connect user
        [HttpGet("user-info")]
        [Authorize]  
        public async Task<ActionResult<FinancierDto>> GetCurrentUserInfo()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

            if (currentUser == null)
            {
                return NotFound();
            }

            var userInfoDto = new FinancierDto
            {
                Username = currentUser.UserName,
                Nom = currentUser.Nom,  
                Prenom = currentUser.Prenom,
                Cin = currentUser.Cin,
                Password = currentUser.PasswordHash,
                Addresse = currentUser.Addresse,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber
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

            // In production: Send token by email
            var resetLink = $"{config["App:ClientUrl"]}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            // For testing/demo purposes only:
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
    

