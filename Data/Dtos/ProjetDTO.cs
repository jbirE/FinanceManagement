using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Data.Dtos
{
    public class ProjetDto
    {
        public int IdProjet { get; set; }

        [Required]
        public string Nom { get; set; }

        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; }

        [Required]
        public int DepartementId { get; set; }

        public string DepartementNom { get; set; }

        public string ResponsableId { get; set; }

        public string ResponsableNom { get; set; }

        public double MontantAlloueTotal { get; set; }

        [Required]
        public string Status { get; set; }
    }
}