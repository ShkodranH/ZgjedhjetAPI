using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ESQuery = Elastic.Clients.Elasticsearch.QueryDsl.Query;
using Shkodran_Hasani_Zgjedhjet_API.Models.Elastic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Shkodran_Hasani_Zgjedhjet_API.Services.Implementations
{
    public class ElasticService<T> : IElasticService<T> where T : class
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public ElasticService(ElasticsearchClient client, string indexName)
        {
            _client = client;
            _indexName = indexName;
        }

        public async Task IndexDocumentAsync(T document)
        {
            await _client.IndexAsync(document, i => i.Index(_indexName));
        }

        public async Task IndexManyAsync(IEnumerable<T> documents)
        {
            await _client.BulkAsync(b => b.Index(_indexName).IndexMany(documents));
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _client.DeleteAsync<T>(id, d => d.Index(_indexName));
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<T>(id, g => g.Index(_indexName));
            return response.Source;
        }


        public async Task<IEnumerable<T>> SearchAsync(string? komuna, string? partia, string? kategoria)
        {
            var filters = new List<ESQuery>();

            if (!string.IsNullOrWhiteSpace(komuna))
                filters.Add(new TermQuery { Field = "Komuna.keyword", Value = komuna });

            if (!string.IsNullOrWhiteSpace(partia))
                filters.Add(new TermQuery { Field = "Partia.keyword", Value = partia });

            if (!string.IsNullOrWhiteSpace(kategoria))
                filters.Add(new TermQuery { Field = "Kategoria.keyword", Value = kategoria });

            var searchRequest = new SearchRequest<T>(_indexName)
            {
                Size = 100,
                Query = filters.Any()
                    ? new BoolQuery { Filter = filters }
                    : new MatchAllQuery()
            };

            var response = await _client.SearchAsync<T>(searchRequest);

            if (!response.IsValidResponse)
            {
                throw new Exception($"Elasticsearch query failed: {response.DebugInformation}");
            }

            return response.Documents;
        }



        public async Task<IEnumerable<string>> SuggestKomunaAsync(string query, int top = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<string>();

            var response = await _client.SearchAsync<T>(s => s
                .Indices(_indexName)
                .Size(top * 5) 
                .Query(q => q.MatchPhrasePrefix(m => m
                    .Field("Komuna")
                    .Query(query)
                ))
            );

            var suggestions = response.Hits
                .Select(h => (h.Source as ZgjedhjeElasticDocument).Komuna.ToString())
                .Distinct()
                .Take(top);

            return suggestions;
        }
    }
}
