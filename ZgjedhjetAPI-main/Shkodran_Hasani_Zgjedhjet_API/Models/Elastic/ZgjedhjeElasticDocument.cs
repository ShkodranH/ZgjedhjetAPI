namespace Shkodran_Hasani_Zgjedhjet_API.Models.Elastic
{
    public class ZgjedhjeElasticDocument
    {
        public int Id { get; set; }
        public string Komuna { get; set; }
        public string KomunaKeyword => Komuna;

        public string Partia { get; set; }
        public string PartiaKeyword => Partia;

        public string Kategoria { get; set; }
        public string KategoriaKeyword => Kategoria;

        public string QendraEVotimit { get; set; }
        public string Vendvotimi { get; set; }
        public int Vota { get; set; }
        public DateTime DataZgjedhjes { get; set; }
    }
}
