using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object? data = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            // Forward the API's session cookie so the API knows who is logged in
            var apiCookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiCookie");
            if (!string.IsNullOrEmpty(apiCookie))
                request.Headers.Add("Cookie", apiCookie);

            return await _httpClient.SendAsync(request);
        }

        public Task<HttpResponseMessage> GetAsync(string endpoint) => SendAsync(HttpMethod.Get, endpoint);
        public Task<HttpResponseMessage> PostAsync(string endpoint, object data) => SendAsync(HttpMethod.Post, endpoint, data);
        public Task<HttpResponseMessage> PutAsync(string endpoint, object data) => SendAsync(HttpMethod.Put, endpoint, data);
        public Task<HttpResponseMessage> DeleteAsync(string endpoint) => SendAsync(HttpMethod.Delete, endpoint);
    }
}

