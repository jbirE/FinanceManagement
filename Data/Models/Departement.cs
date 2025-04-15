using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Data.Models
{



public partial class Departement
{
        [Key]

        public int IdDepartement { get; set; }

    public string Name { get; set; }

    public double BudgetTotal { get; set; }



    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public virtual ICollection<Projet> Projets { get; set; } = new List<Projet>();

}
}
