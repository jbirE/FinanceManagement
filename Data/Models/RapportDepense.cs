using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceManagement.Data.Models;

public class RapportDepense
{
    [Key]
    public int IdRpport { get; set; }

    public double Montant { get; set; }
    public DateTime DateSoumission { get; set; }
    public StatutRapport StatutApprobation { get; set; }
    public required string  Description { get; set; }

    [ForeignKey("BudgetProjet")]
    public int BudgetProjetId { get; set; }
    public BudgetProjet BudgetProjet { get; set; }

    [ForeignKey("Utilisateur")]
    public string UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }

    public ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public enum StatutRapport : short
    {
        approuve,
        rejete, 
        enTraitement, // manager va lancer l'etude du rapport depenses // l'employe ne peut pas modifier le rapport
        soumis // peut modifier le repport 

    }
}