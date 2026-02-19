namespace Shkodran_Hasani_Zgjedhjet_API.Services
{
    public interface IElasticService<T>
    {
        Task IndexDocumentAsync(T document);
        Task IndexManyAsync(IEnumerable<T> documents);
        Task DeleteDocumentAsync(string id);
        Task<T?> GetByIdAsync(string id);

        Task<IEnumerable<T>> SearchAsync(string? komuna, string? partia, string? kategoria);
        Task<IEnumerable<string>> SuggestKomunaAsync(string query, int top = 5);
    }
}
