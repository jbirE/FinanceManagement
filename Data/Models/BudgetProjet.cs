using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models;

public class BudgetProjet
{
    [Key]
    public int IdBudgetProjet { get; set; }

    [Range(0.01, double.MaxValue)]
    public double MontantAlloue { get; set; }

    public double DepensesTotales { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime DateFinProjet { get; set; } = DateTime.Now;

    [ForeignKey("Projet")]
    public int ProjetId { get; set; }
    public Projet Projet { get; set; }

    [ForeignKey("Utilisateur")]
    public string UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    public ICollection<RapportDepense> Rapports { get; set; } = new List<RapportDepense>();
}