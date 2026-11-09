using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Services
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

        private void AddSessionHeaders()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserID");
            var role = _httpContextAccessor.HttpContext?.Session.GetString("Role");
            if (!string.IsNullOrEmpty(userId))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
                _httpClient.DefaultRequestHeaders.Remove("X-User-Role");
                _httpClient.DefaultRequestHeaders.Add("X-User-Role", role);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AddSessionHeaders();
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        // This now returns T directly, not HttpResponseMessage!
        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            AddSessionHeaders();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default; // Returns null if failed
        }

        public async Task<bool> PostNoReturnAsync(string endpoint, object data)
        {
            AddSessionHeaders();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<T?> UploadFileAsync<T>(string endpoint, IFormFile file)
        {
            AddSessionHeaders();
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.FileName);

            var response = await _httpClient.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }
    }
}