using Microsoft.AspNetCore.Identity;
using System.Data.Common;

namespace FinanceManagement.Data.Models
{
    public class Utilisateur :IdentityUser
    {
        //plusieur attributs exist et generes par identity : ID, Email, userName, phoneNumber, pwd
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Addresse { get; set; }
        public string Cin { get; set; }
        public bool EmailConfirmed { get; set; } // For email confirmation
        public ICollection<RapportDepense> Rapports { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}