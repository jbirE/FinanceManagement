using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FinanceManagement.Data.Models
{
    public class RapportDepense
    {
        [Key]
        public int IdRpport { get; set; }

        public double Montant { get; set; }

        public DateTime DateSoumission { get; set; }

        public string StatutApprobation { get; set; }

        public string Description { get; set; }

        [ForeignKey("Budget")]

        public int BudgetId { get; set; }
        public Budget Budget { get; set; }


        [ForeignKey("Projet")]

        public int? ProjetId { get; set; }
        [JsonIgnore]
        public Projet Projet { get; set; }

        [ForeignKey("Utilisateur")]
        public string UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public string ExpenseCategory { get; set; } // finance, transport , material
        public ICollection<Facture> Factures { get; set; }

    }
}
