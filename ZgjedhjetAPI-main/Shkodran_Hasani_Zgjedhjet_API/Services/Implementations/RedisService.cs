using StackExchange.Redis;
using System.Text.Json;

namespace Shkodran_Hasani_Zgjedhjet_API.Services.Implementations
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;
        private const string SuggestionKey = "komuna:suggestions";

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }


        public async Task IncrementSuggestionStats(IEnumerable<string> suggestions)
        {
            var tasks = new List<Task>();

            foreach (var komuna in suggestions)
            {
                if (!string.IsNullOrWhiteSpace(komuna))
                {
                    tasks.Add(_db.SortedSetIncrementAsync(SuggestionKey, komuna, 1));
                }
            }

            await Task.WhenAll(tasks);
        }


        public async Task<IEnumerable<(string Komuna, int Count)>> GetTopSuggestions(int top)
        {
            var entries = await _db.SortedSetRangeByRankWithScoresAsync(
                SuggestionKey,
                0,
                top - 1,
                Order.Descending
            );

            return entries.Select(e => (e.Element.ToString()!, (int)e.Score));
        }
    }
}
