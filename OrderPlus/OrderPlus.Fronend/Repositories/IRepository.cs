namespace OrderPlus.Fronend.Repositories
{
    public interface IRepository
    {
        Task<HttpResponseWrapper<T>> GetAsync<T>(string Url);
        Task<HttpResponseWrapper<object>> PostAsync<T>(string Url, T model);
        Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string Url, T model);
        Task<HttpResponseWrapper<object>> DeleteAsync<T>(string Url);
        Task<HttpResponseWrapper<object>> PutAsync<T>(string Url, T model);
        Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string Url, T model);
        Task<HttpResponseWrapper<object>> GetAsync(string Url);

    }
}
