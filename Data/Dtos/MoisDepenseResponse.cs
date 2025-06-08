namespace FinanceManagement.Data.Dtos
{
    public class MoisDepenseResponse
    {
        public string Mois { get; set; }
        public int MoisNumero { get; set; }
        public int Annee { get; set; }
        public double TotalDepenses { get; set; }
        public int NombreRapports { get; set; }
    }
}
