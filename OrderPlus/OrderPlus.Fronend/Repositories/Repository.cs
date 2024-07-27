
using System.Text;
using System.Text.Json;

namespace OrderPlus.Fronend.Repositories
{
    public class Repository : IRepository
    {
       
        private JsonSerializerOptions _jsonDefaultOptions=new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        private readonly HttpClient _httpClient;

        public Repository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string Url)
        {
            var responseHTTP = await _httpClient.GetAsync(Url);
            if(responseHTTP.IsSuccessStatusCode)
            {
                var response=await UnserializeAnswerAsync<T>(responseHTTP);
                return new HttpResponseWrapper<T>(response, false, responseHTTP);
            }
            return new HttpResponseWrapper<T>(default, true, responseHTTP);
        }

        public async Task<HttpResponseWrapper<object>> GetAsync(string Url)
        {
            var responseHTTP = await _httpClient.GetAsync(Url);
            return new HttpResponseWrapper<object>(null, !responseHTTP.IsSuccessStatusCode, responseHTTP);
        }

        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string Url, T model)
        {
            var messageJson=JsonSerializer.Serialize(model);
            var messageContent=new StringContent(messageJson, Encoding.UTF8, "application/json");
            var responseHttp=await _httpClient.PostAsync(Url, messageContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string Url, T model)
        {
            var messageJson = JsonSerializer.Serialize(model);
            var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PostAsync(Url, messageContent);
            if(responseHttp.IsSuccessStatusCode)
            {
                var response=await UnserializeAnswerAsync<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }
            return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
        }

        public async Task<HttpResponseWrapper<object>> PutAsync<T>(string Url, T model)
        {
            var messageJson = JsonSerializer.Serialize(model);
            var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PutAsync(Url, messageContent);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public async Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string Url, T model)
        {
            var messageJson = JsonSerializer.Serialize(model);
            var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PutAsync(Url, messageContent);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswerAsync<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }
            return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
        }
        public async Task<HttpResponseWrapper<object>> DeleteAsync<T>(string Url)
        {
            var responseHttp=await _httpClient.DeleteAsync(Url);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp); 
        }

        private async Task<T> UnserializeAnswerAsync<T>(HttpResponseMessage httpResponseMessage)
        {
            var response =await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!;
        }
    }
}
