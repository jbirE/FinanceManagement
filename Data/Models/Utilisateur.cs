using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FinanceManagement.Data.Models
{
    public class Utilisateur : IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Addresse { get; set; }
        public string Cin { get; set; }
        public DateTime dateEmbauche { get; set; }
        public DateTime DerniereConnexion { get; set; } 
        public bool Status { get; set; }

        [ForeignKey("Departement")]
        public int IdDepartement { get; set; }
        public Departement Departement { get; set; }

        public ICollection<RapportDepense> Rapports { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}