using FinanceManagement.Data.Enum;

namespace FinanceManagement.Data.Dtos
{
    public class NotificationDto
    {
        public int IdNotif { get; set; }
        public string Message { get; set; }
        public string Titre { get; set; }
        public DateTime DateCreation { get; set; }
        public bool IsReaded { get; set; }
        public int EntityId { get; set; }
        public TypeNotification TypeEntityNotification { get; set; }
        public string DestinataireId { get; set; }
        public string DestinataireNom { get; set; }
        public string DateCreationFormatted => DateCreation.ToString("dd/MM/yyyy HH:mm");
        public string TempsEcoule => GetTempsEcoule();

        private string GetTempsEcoule()
        {
            var now = DateTime.Now;
            var diff = now - DateCreation;

            if (diff.TotalMinutes < 1)
                return "À l'instant";
            if (diff.TotalMinutes < 60)
                return $"Il y a {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24)
                return $"Il y a {(int)diff.TotalHours}h";
            if (diff.TotalDays < 7)
                return $"Il y a {(int)diff.TotalDays} jour(s)";

            return DateCreation.ToString("dd/MM/yyyy");
        }
    }
}
