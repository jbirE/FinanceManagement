using FinanceManagement.Data.Dtos;
using FinanceManagement.Repositories.Interface;
using FinanceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize] // Nécessite une authentification
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IUserContext  _usuContext;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger, IUserContext usuContext)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _usuContext = usuContext;
        }

        /// <summary>
        /// Récupère l'état global du budget du département pour une année donnée
        /// </summary>
        /// <param name="annee">Année pour laquelle récupérer les données</param>
        /// <returns>État du budget départemental</returns>
        [HttpGet("etat-budget/{annee:int}")]
        public async Task<ActionResult<EtatBudgetResponse>> GetEtatBudgetParAnnee(int annee)
        {
            try
            {
                
                if (annee < 2000 || annee > DateTime.Now.Year + 5)
                    return BadRequest("Année invalide");

                var result = await _dashboardService.GetEtatBudgetParAnneeAsync(annee);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'état du budget pour l'année {Annee}", annee);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère les détails des projets du département pour une année donnée
        /// </summary>
        /// <param name="annee">Année pour laquelle récupérer les projets</param>
        /// <returns>Liste des détails des projets</returns>
        [HttpGet("details-projets/{annee:int}")]
        public async Task<ActionResult<List<DetailProjetResponse>>> GetDetailsProjets(int annee)
        {
            try
            {
           
                if (annee < 2000 || annee > DateTime.Now.Year + 5)
                    return BadRequest("Année invalide");

                var result = await _dashboardService.GetDetailsProjetsAsync(annee);
                return Ok(result);
            }
       
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des détails des projets pour l'année {Annee}", annee);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère les dépenses par mois pour une année donnée
        /// </summary>
        /// <param name="annee">Année pour laquelle récupérer les dépenses mensuelles</param>
        /// <returns>Liste des dépenses par mois</returns>
        [HttpGet("depenses-mensuelles/{annee:int}")]
        public async Task<ActionResult<List<MoisDepenseResponse>>> GetDepensesParMois(int annee)
        {
            try
            {

                return Unauthorized("Utilisateur non authentifié");

                if (annee < 2000 || annee > DateTime.Now.Year + 5)
                    return BadRequest("Année invalide");

                var result = await _dashboardService.GetDepensesParMoisAsync(annee);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des dépenses mensuelles pour l'année {Annee}", annee);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère les KPI de trésorerie pour une année donnée
        /// </summary>
        /// <param name="annee">Année pour laquelle calculer les KPI</param>
        /// <returns>KPI de trésorerie</returns>
        [HttpGet("kpi-tresorerie/{annee:int}")]
        public async Task<ActionResult<KPITresorerieResponse>> GetKPITresorerie(int annee)
        {
            try
            {
              

                if (annee < 2000 || annee > DateTime.Now.Year + 5)
                    return BadRequest("Année invalide");

                var result = await _dashboardService.GetKPITresorerieAsync(annee);
                return Ok(result);
            }
       
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul des KPI de trésorerie pour l'année {Annee}", annee);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère un résumé complet du tableau de bord pour une année donnée
        /// </summary>
        /// <param name="annee">Année pour laquelle récupérer le résumé</param>
        /// <returns>Résumé complet du tableau de bord</returns>
        [HttpGet("resume-complet/{annee:int}")]
        public async Task<ActionResult<DashboardResumeResponse>> GetResumeComplet(int annee)
        {
            try
            {
                

                if (annee < 2000 || annee > DateTime.Now.Year + 5)
                    return BadRequest("Année invalide");

                // Appel parallèle de toutes les méthodes pour optimiser les performances
                var tasks = new Task[]
                {
                    _dashboardService.GetEtatBudgetParAnneeAsync(annee),
                    _dashboardService.GetDetailsProjetsAsync(annee),
                    _dashboardService.GetDepensesParMoisAsync(annee),
                    _dashboardService.GetKPITresorerieAsync(annee)
                };

                await Task.WhenAll(tasks);

                var resume = new DashboardResumeResponse
                {
                    EtatBudget = (EtatBudgetResponse)((Task<EtatBudgetResponse>)tasks[0]).Result,
                    DetailsProjets = (List<DetailProjetResponse>)((Task<List<DetailProjetResponse>>)tasks[1]).Result,
                    DepensesMensuelles = (List<MoisDepenseResponse>)((Task<List<MoisDepenseResponse>>)tasks[2]).Result,
                    KPITresorerie = (KPITresorerieResponse)((Task<KPITresorerieResponse>)tasks[3]).Result,
                    DateGeneration = DateTime.Now
                };

                return Ok(resume);
            }
      
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du résumé complet pour l'année {Annee}", annee);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère la liste des années disponibles pour le département de l'utilisateur
        /// </summary>
        /// <returns>Liste des années avec des données</returns>
        [HttpGet("annees-disponibles")]
        public async Task<ActionResult<List<int>>> GetAnneesDisponibles()
        {
            try
            {
                var user = _usuContext.Get();
                
                var annees = new List<int>();
                var anneeActuelle = DateTime.Now.Year;

                for (int i = anneeActuelle - 4; i <= anneeActuelle; i++)
                {
                    annees.Add(i);
                }

                return Ok(annees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des années disponibles");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}