using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagement.Data.Models
{



public partial class Projet
{
        [Key]
    public int IdProjet { get; set; }

    public string Nom { get; set; }

    public double BudgetAlloue { get; set; }

    public DateTime DateDebut { get; set; }

    public DateTime? DateFin { get; set; }
        [ForeignKey("Departement")]

        public int DepartementId { get; set; }

        public virtual Departement Departement { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        [ForeignKey("Utilisateur")]

        public string? ResponsableId { get; set; }
        public Utilisateur Responsable { get; set; }
        public ICollection<RapportDepense> Rapports { get; set; }


    }
}