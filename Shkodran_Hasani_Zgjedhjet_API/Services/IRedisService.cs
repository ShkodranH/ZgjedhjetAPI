namespace Shkodran_Hasani_Zgjedhjet_API.Services
{
    public interface IRedisService
    {
        Task IncrementSuggestionStats(IEnumerable<string> suggestions);
        Task<IEnumerable<(string Komuna, int Count)>> GetTopSuggestions(int top);
    }
}
