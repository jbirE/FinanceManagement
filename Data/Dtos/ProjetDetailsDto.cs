namespace FinanceManagement.Data.Dtos
{
    public class ProjetDetailsDto
    {
        public string NomProjet { get; set; }
        public double PourcentageCompletion { get; set; }
        public double MontantAlloue { get; set; }
        public double TotalDepenses { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
