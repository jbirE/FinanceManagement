using FinanceManagement.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Data.Dtos
{
    public class RapportDepenseDTO
    {
        internal readonly int IdRapport;

        public int IdRpport { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        public double Montant { get; set; }

        public DateTime DateSoumission { get; set; }

        public RapportDepense.StatutRapport StatutApprobation { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "La description doit contenir entre 10 et 500 caractères")]
        public string Description { get; set; }

        [Required]
        public int BudgetProjetId { get; set; }

        public string BudgetProjetNom { get; set; }

        public string UtilisateurId { get; set; }

        public string UtilisateurNom { get; set; }
        public enum StatutRapport : short
    {
        Approuve,
        Rejete,
        EnAttente
    }

    }
}