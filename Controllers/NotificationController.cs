using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinanceManagement.Repositories.Interface;
using FinanceManagement.Services;
using Microsoft.IdentityModel.Tokens;

namespace FinanceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize] // Assuming you want authentication
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;
        private readonly IUserContext _userContext;
        public NotificationController(
            NotificationService notificationService,
            IUserContext userContext,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
            _userContext = userContext;
        }

        [HttpPut("{id}/marquer-comme-vue")]
        public async Task<IActionResult> MarquerCommeVue(int id)
        {
            try
            {
                await _notificationService.MarquerCommeVuAsync(id);
                return Ok(new { message = "Notification marquée comme vue avec succès" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Notification non trouvée: {NotificationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de la notification {NotificationId} comme vue", id);
                return StatusCode(500, new { message = "Une erreur interne s'est produite" });
            }
        }

        [HttpPut("marquer-plusieurs-comme-vues")]
        public async Task<IActionResult> MarquerPlusieursCommeVues([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest(new { message = "Aucun ID de notification fourni" });
            }

            try
            {
                var results = new List<object>();

                foreach (var id in ids)
                {
                    try
                    {
                        await _notificationService.MarquerCommeVuAsync(id);
                        results.Add(new { id, success = true });
                    }
                    catch (ArgumentException)
                    {
                        results.Add(new { id, success = false, error = "Notification non trouvée" });
                    }
                }

                return Ok(new { message = "Traitement terminé", results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de plusieurs notifications comme vues");
                return StatusCode(500, new { message = "Une erreur interne s'est produite" });
            }
        }

        [HttpGet("count-non-lues")]
        public async Task<IActionResult> GetCountNotificationsNonLues()
        {
            try
            {
                // Assuming you have a way to get current user ID
                var user = await _userContext.Get();
                if (user != null)
                {
                    return Unauthorized(new { message = "Utilisateur non authentifié" });
                }

                var count = await _notificationService.GetCountNotificationsNonLuesAsync(user.Id);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de notifications non lues");
                return StatusCode(500, new { message = "Une erreur interne s'est produite" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool nonLuesSeules = false)
        {
            try
            {
                var user = await _userContext.Get();
                if (user == null)
                {
                    return Unauthorized(new { message = "Utilisateur non authentifié" });
                }

                var notifications = await _notificationService.GetNotificationsAsync(
                    user.Id, page, pageSize, nonLuesSeules);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des notifications");
                return StatusCode(500, new { message = "Une erreur interne s'est produite" });
            }
        }
    }
}