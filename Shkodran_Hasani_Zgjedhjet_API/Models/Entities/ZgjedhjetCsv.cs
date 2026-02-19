namespace Shkodran_Hasani_Zgjedhjet_API.Models.Entities
{
    public class ZgjedhjetCsv
    {
        public string Kategoria { get; set; } = string.Empty;
        public string Komuna { get; set; } = string.Empty;
        public string Qendra_e_Votimit { get; set; } = string.Empty;
        public string VendVotimi { get; set; } = string.Empty;
        public string Partia { get; set; } = string.Empty;
        public int Vota { get; set; }
    }
}
