using FinanceManagement.Data.Dtos;
using FinanceManagement.Data.Models;
using FinanceManagement.DbSql;
using FinanceManagement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceManagement.Services
{

    public class DashboardService : IDashboardService
    {
        private readonly DataContext _context;
        private readonly IUserContext _userContext;

        public DashboardService(DataContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<EtatBudgetResponse> GetEtatBudgetParAnneeAsync(int annee)
        {
            // Charger l'utilisateur 
            var user = await _userContext.Get();      

            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur non trouvé");

            // Budget département pour l'année
            var budgetDepartement = await _context.BudgetsDepartements
                .FirstOrDefaultAsync(bd => bd.DepartementId == user.IdDepartement && bd.Annee == annee);

            if (budgetDepartement == null)
                return new EtatBudgetResponse
                {
                    Annee = annee,
                    NomDepartement = user.Departement.Name,
                    BudgetDepartementAnnuel = 0,
                    TotalMontantAlloue = 0,
                    TotalDepenses = 0,
                    SoldeDisponible = 0
                };

            // Calculer les totaux des projets du département
            var projets = await _context.Projets
                .Where(p => p.DepartementId == user.IdDepartement)
                .Include(p => p.BudgetsProjets.Where(bp => bp.DateCreation.Year == annee))
                .ToListAsync();

            var totalMontantAlloue = projets
                .SelectMany(p => p.BudgetsProjets)
                .Sum(bp => bp.MontantAlloue);

            var totalDepenses = projets
                .SelectMany(p => p.BudgetsProjets)
                .Sum(bp => bp.DepensesTotales);

            return new EtatBudgetResponse
            {
                Annee = annee,
                NomDepartement = user.Departement.Name,
                BudgetDepartementAnnuel = budgetDepartement.MontantAnnuel,
                TotalMontantAlloue = totalMontantAlloue,
                TotalDepenses = totalDepenses,
                SoldeDisponible = budgetDepartement.MontantAnnuel - totalMontantAlloue,
                PourcentageUtilisation = budgetDepartement.MontantAnnuel > 0
                    ? (totalMontantAlloue / budgetDepartement.MontantAnnuel) * 100
                    : 0
            };
        }

        public async Task<List<DetailProjetResponse>> GetDetailsProjetsAsync(int annee)
        {
            // Charger l'utilisateur 
            var user = await _userContext.Get();
            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur non trouvé");

            // Récupérer tous les projets du département avec leurs budgets
            var projets = await _context.Projets
                .Where(p => p.DepartementId == user.IdDepartement)
                .Include(p => p.BudgetsProjets.Where(bp => bp.DateCreation.Year == annee))
                .ToListAsync();

            var detailsProjets = new List<DetailProjetResponse>();

            foreach (var projet in projets)
            {
                var budgetProjet = projet.BudgetsProjets.FirstOrDefault();

                if (budgetProjet != null)
                {
                    var pourcentageCompletion = CalculerPourcentageCompletion(projet, budgetProjet);

                    detailsProjets.Add(new DetailProjetResponse
                    {
                        NomProjet = projet.Nom,
                        PourcentageCompletion = pourcentageCompletion,
                        MontantAlloue = budgetProjet.MontantAlloue,
                        TotalDepenses = budgetProjet.DepensesTotales,
                        SoldeRestant = budgetProjet.MontantAlloue - budgetProjet.DepensesTotales,
                        DateDebut = projet.DateDebut,
                        DateFin = projet.DateFin,
                        StatutBudget = budgetProjet.DepensesTotales > budgetProjet.MontantAlloue ? "Dépassé" : "Normal"
                    });
                }
            }

            return detailsProjets.OrderByDescending(d => d.MontantAlloue).ToList();
        }

        public async Task<List<MoisDepenseResponse>> GetDepensesParMoisAsync(int annee)
        {
            // Charger l'utilisateur 
            var user = await _userContext.Get();

            if (user == null)
                throw new UnauthorizedAccessException("Utilisateur non trouvé");

            // Récupérer tous les budgets projets du département pour l'année
            var budgetsProjets = await _context.BudgetsProjets
                .Include(bp => bp.Projet)
                .Include(bp => bp.Rapports.Where(r => r.StatutApprobation == RapportDepense.StatutRapport.approuve))
                .Where(bp => bp.Projet.DepartementId == user.IdDepartement &&
                           bp.DateCreation.Year == annee)
                .ToListAsync();

            var moisDepenses = new List<MoisDepenseResponse>();

            // Générer la liste des mois depuis la création du premier budget projet
            var dateDebut = budgetsProjets.Any() ? budgetsProjets.Min(bp => bp.DateCreation) : new DateTime(annee, 1, 1);
            var dateFin = new DateTime(annee, 12, 31);

            for (var date = new DateTime(dateDebut.Year, dateDebut.Month, 1);
                 date <= dateFin;
                 date = date.AddMonths(1))
            {
                var depensesMois = budgetsProjets
                    .SelectMany(bp => bp.Rapports)
                    .Where(r => r.DateSoumission.Year == date.Year &&
                               r.DateSoumission.Month == date.Month &&
                               r.StatutApprobation == RapportDepense.StatutRapport.approuve)
                    .Sum(r => r.Montant);

                moisDepenses.Add(new MoisDepenseResponse
                {
                    Mois = date.ToString("MMMM yyyy"),
                    MoisNumero = date.Month,
                    Annee = date.Year,
                    TotalDepenses = depensesMois,
                    NombreRapports = budgetsProjets
                        .SelectMany(bp => bp.Rapports)
                        .Count(r => r.DateSoumission.Year == date.Year &&
                                   r.DateSoumission.Month == date.Month &&
                                   r.StatutApprobation == RapportDepense.StatutRapport.approuve)
                });
            }

            return moisDepenses.OrderBy(m => m.MoisNumero).ToList();
        }

        public async Task<KPITresorerieResponse> GetKPITresorerieAsync(int annee)
        {
          
            var etatBudget = await GetEtatBudgetParAnneeAsync(annee);

            // Calcul des KPI de trésorerie
            var tresorerieNette = etatBudget.BudgetDepartementAnnuel - etatBudget.TotalDepenses;
            var ratioLiquidite = etatBudget.BudgetDepartementAnnuel > 0
                ? (etatBudget.SoldeDisponible / etatBudget.BudgetDepartementAnnuel) * 100
                : 0;

            // Récupérer les données de l'année précédente pour comparaison
            var etatBudgetPrecedent = await GetEtatBudgetParAnneeAsync(annee - 1);
            var variationAnnuelle = etatBudgetPrecedent.TotalDepenses > 0
                ? ((etatBudget.TotalDepenses - etatBudgetPrecedent.TotalDepenses) / etatBudgetPrecedent.TotalDepenses) * 100
                : 0;

            return new KPITresorerieResponse
            {
                Annee = annee,
                TresorerieNette = tresorerieNette,
                RatioLiquidite = ratioLiquidite,
                VariationAnnuelle = variationAnnuelle,
                BudgetTotal = etatBudget.BudgetDepartementAnnuel,
                DepensesTotales = etatBudget.TotalDepenses,
                StatutTresorerie = tresorerieNette >= 0 ? "Positive" : "Négative",
                RecommandationAlerte = tresorerieNette < 0 ? "Attention: Trésorerie négative détectée" :
                                     ratioLiquidite < 20 ? "Attention: Liquidité faible" : "Situation normale"
            };
        }

        private double CalculerPourcentageCompletion(Projet projet, BudgetProjet budgetProjet)
        {
            // Calcul basé sur le temps écoulé et les dépenses
            var maintenant = DateTime.Now;
            var dureeProjet = (projet.DateFin ?? budgetProjet.DateFinProjet) - projet.DateDebut;
            var tempsEcoule = maintenant - projet.DateDebut;

            var pourcentageTemps = dureeProjet.TotalDays > 0
                ? Math.Min(100, (tempsEcoule.TotalDays / dureeProjet.TotalDays) * 100)
                : 0;

            var pourcentageBudget = budgetProjet.MontantAlloue > 0
                ? (budgetProjet.DepensesTotales / budgetProjet.MontantAlloue) * 100
                : 0;

            // Moyenne pondérée (60% temps, 40% budget)
            return Math.Round((pourcentageTemps * 0.6) + (pourcentageBudget * 0.4), 2);
        }

    }

}