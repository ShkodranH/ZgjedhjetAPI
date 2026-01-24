namespace Shkodran_Hasani_Zgjedhjet_API.Models.DTOs
{
    public class ZgjedhjetAggregatedResponse
    {
        public List<PartiaVotesResponse> Results { get; set; } = new();
    }
}
